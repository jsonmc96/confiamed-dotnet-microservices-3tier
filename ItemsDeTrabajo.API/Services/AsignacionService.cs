using ItemsDeTrabajo.API.Models;
using ItemsDeTrabajo.API.Repositories;

namespace ItemsDeTrabajo.API.Services;

/// <summary>
/// Servicio de asignación de ítems de trabajo.
/// Implementa el algoritmo de asignación basado en las reglas de negocio de CONFIAMED.
///
/// REGLAS (por orden de prioridad):
///   1. URGENCIA: Si la fecha de entrega es en 2 días o menos,
///      asignar al usuario con MENOS ÍTEMS TOTALES (sin importar relevancia).
///   2. RELEVANCIA: Si el ítem es relevante,
///      asignar al usuario con MENOS ÍTEMS PENDIENTES.
///   3. GENERAL: Asignar al usuario con menos ítems totales.
///
/// CASO DE PRUEBA DEL DOCUMENTO:
///   - Usuario A: 3 ítems (2 relevantes)
///   - Usuario B: 1 ítem (0 relevantes)
///   - Nuevo ítem: altamente relevante, entrega en 2 días
///   → Se asigna a Usuario B (aplica Regla 1 por urgencia; B tiene menos ítems totales)
/// </summary>
public class AsignacionService : IAsignacionService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<AsignacionService> _logger;

    // Umbral de días para considerar que la fecha de entrega es urgente
    private const int UmbralDiasUrgente = 2;

    public AsignacionService(
        IUsuarioRepository usuarioRepository,
        IItemRepository itemRepository,
        ILogger<AsignacionService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _itemRepository = itemRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AsignacionResult> AsignarItemAsync(AsignacionRequest request)
    {
        var (usuario, regla) = await EvaluarAlgoritmoAsignacion(request);

        if (usuario == null)
        {
            return new AsignacionResult
            {
                Exitoso = false,
                Mensaje = "No hay usuarios disponibles para asignar el ítem."
            };
        }

        // Crear y persistir el ítem asignado
        var nuevoItem = new ItemTrabajo
        {
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            EsRelevante = request.EsRelevante,
            FechaEntrega = request.FechaEntrega,
            UsuarioAsignadoId = usuario.Id,
            Estado = EstadoItem.Pendiente
        };

        var itemCreado = await _itemRepository.CrearAsync(nuevoItem);

        _logger.LogInformation(
            "Ítem '{Titulo}' asignado a usuario {UserId} ({NombreUsuario}). Regla aplicada: {Regla}",
            request.Titulo, usuario.Id, usuario.Nombre, regla);

        return new AsignacionResult
        {
            Exitoso = true,
            Mensaje = $"Ítem asignado exitosamente a {usuario.Nombre}.",
            UsuarioAsignadoId = usuario.Id,
            NombreUsuario = usuario.Nombre,
            ReglaAplicada = regla,
            ItemCreado = itemCreado
        };
    }

    /// <inheritdoc />
    public async Task<AsignacionResult> SimularAsignacionAsync(AsignacionRequest request)
    {
        var (usuario, regla) = await EvaluarAlgoritmoAsignacion(request);

        if (usuario == null)
        {
            return new AsignacionResult
            {
                Exitoso = false,
                Mensaje = "No hay usuarios disponibles para asignar el ítem."
            };
        }

        return new AsignacionResult
        {
            Exitoso = true,
            Mensaje = $"[SIMULACIÓN] El ítem se asignaría a {usuario.Nombre}.",
            UsuarioAsignadoId = usuario.Id,
            NombreUsuario = usuario.Nombre,
            ReglaAplicada = regla
        };
    }

    // -------------------------------------------------------------------------
    // ALGORITMO CENTRAL DE ASIGNACIÓN
    // -------------------------------------------------------------------------

    /// <summary>
    /// Núcleo del algoritmo. Evalúa las reglas de negocio en orden de prioridad
    /// y devuelve el usuario seleccionado junto con la regla que determinó la elección.
    /// </summary>
    private async Task<(Usuario? usuario, string regla)> EvaluarAlgoritmoAsignacion(
        AsignacionRequest request)
    {
        var usuarios = (await _usuarioRepository.ObtenerTodosConItemsAsync()).ToList();

        if (usuarios.Count == 0)
            return (null, string.Empty);

        // Calcular días restantes hasta la fecha de entrega
        var diasRestantes = (request.FechaEntrega.Date - DateTime.UtcNow.Date).Days;

        _logger.LogDebug(
            "Evaluando asignación: EsRelevante={EsRelevante}, DiasRestantes={DiasRestantes}",
            request.EsRelevante, diasRestantes);

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // REGLA 1 — URGENCIA: fecha de entrega ≤ umbral (2 días)
        // Asignar al usuario con MENOS ÍTEMS TOTALES, ignorando la relevancia.
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        if (diasRestantes <= UmbralDiasUrgente)
        {
            var usuarioMenosItems = usuarios
                .OrderBy(u => u.TotalItems)
                .ThenBy(u => u.Id)           // Desempate: usuario con ID menor
                .First();

            const string reglaUrgencia =
                "REGLA 1 - URGENCIA: Fecha de entrega próxima a vencer. " +
                "Asignado al usuario con menos ítems totales.";

            _logger.LogInformation(
                "REGLA 1 (Urgencia) aplicada. Usuario seleccionado: {Id} - {Nombre} ({Total} ítems)",
                usuarioMenosItems.Id, usuarioMenosItems.Nombre, usuarioMenosItems.TotalItems);

            return (usuarioMenosItems, reglaUrgencia);
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // REGLA 2 — RELEVANCIA: ítem relevante con fecha no urgente
        // Asignar al usuario con MENOS ÍTEMS PENDIENTES.
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        if (request.EsRelevante)
        {
            var usuarioMenosPendientes = usuarios
                .OrderBy(u => u.ItemsPendientes)
                .ThenBy(u => u.TotalItems)   // Desempate: menos ítems totales
                .ThenBy(u => u.Id)
                .First();

            const string reglaRelevancia =
                "REGLA 2 - RELEVANCIA: Ítem altamente relevante. " +
                "Asignado al usuario con menos ítems pendientes.";

            _logger.LogInformation(
                "REGLA 2 (Relevancia) aplicada. Usuario seleccionado: {Id} - {Nombre} ({Pendientes} pendientes)",
                usuarioMenosPendientes.Id, usuarioMenosPendientes.Nombre, usuarioMenosPendientes.ItemsPendientes);

            return (usuarioMenosPendientes, reglaRelevancia);
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // REGLA 3 — GENERAL: ítem no urgente y no relevante
        // Asignar al usuario con MENOS ÍTEMS TOTALES.
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        var usuarioGeneral = usuarios
            .OrderBy(u => u.TotalItems)
            .ThenBy(u => u.Id)
            .First();

        const string reglaGeneral =
            "REGLA 3 - GENERAL: Ítem estándar. " +
            "Asignado al usuario con menos ítems totales.";

        _logger.LogInformation(
            "REGLA 3 (General) aplicada. Usuario seleccionado: {Id} - {Nombre} ({Total} ítems)",
            usuarioGeneral.Id, usuarioGeneral.Nombre, usuarioGeneral.TotalItems);

        return (usuarioGeneral, reglaGeneral);
    }
}
