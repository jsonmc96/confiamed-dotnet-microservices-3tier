using ItemsDeTrabajo.API.Models;
using ItemsDeTrabajo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ItemsDeTrabajo.API.Controllers;

/// <summary>
/// Controlador de ítems de trabajo — Capa de Presentación (API).
/// Expone los endpoints REST para gestión y asignación de ítems.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly IAsignacionService _asignacionService;
    private readonly ILogger<ItemsController> _logger;

    public ItemsController(
        IItemService itemService,
        IAsignacionService asignacionService,
        ILogger<ItemsController> logger)
    {
        _itemService = itemService;
        _asignacionService = asignacionService;
        _logger = logger;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CRUD de Ítems
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Obtiene todos los ítems de trabajo registrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ItemTrabajo>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodos()
    {
        var items = await _itemService.ObtenerTodosAsync();
        return Ok(items);
    }

    /// <summary>
    /// Obtiene un ítem de trabajo por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ItemTrabajo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var item = await _itemService.ObtenerPorIdAsync(id);
        if (item == null)
            return NotFound(new { Mensaje = $"Ítem con ID {id} no encontrado." });

        return Ok(item);
    }

    /// <summary>
    /// Obtiene todos los ítems asignados a un usuario específico.
    /// </summary>
    [HttpGet("usuario/{usuarioId:int}")]
    [ProducesResponseType(typeof(IEnumerable<ItemTrabajo>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
    {
        var items = await _itemService.ObtenerPorUsuarioAsync(usuarioId);
        return Ok(items);
    }

    /// <summary>
    /// Crea un nuevo ítem de trabajo sin aplicar el algoritmo de asignación.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ItemTrabajo), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear([FromBody] ItemTrabajo item)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var creado = await _itemService.CrearAsync(item);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
    }

    /// <summary>
    /// Actualiza un ítem de trabajo existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ItemTrabajo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ItemTrabajo item)
    {
        if (id != item.Id)
            return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID del cuerpo." });

        var actualizado = await _itemService.ActualizarAsync(item);
        if (actualizado == null)
            return NotFound(new { Mensaje = $"Ítem con ID {id} no encontrado." });

        return Ok(actualizado);
    }

    /// <summary>
    /// Elimina un ítem de trabajo por su ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _itemService.EliminarAsync(id);
        if (!eliminado)
            return NotFound(new { Mensaje = $"Ítem con ID {id} no encontrado." });

        return NoContent();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Algoritmo de Asignación
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Asigna un nuevo ítem al usuario óptimo aplicando las reglas de negocio:
    /// 1. Urgencia: entrega ≤ 2 días → usuario con menos ítems totales.
    /// 2. Relevancia: ítem relevante → usuario con menos pendientes.
    /// 3. General: usuario con menos ítems totales.
    /// </summary>
    [HttpPost("asignar")]
    [ProducesResponseType(typeof(AsignacionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AsignacionResult), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Asignar([FromBody] AsignacionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation(
            "Solicitud de asignación recibida: '{Titulo}' | Relevante: {EsRelevante} | Entrega: {FechaEntrega}",
            request.Titulo, request.EsRelevante, request.FechaEntrega);

        var resultado = await _asignacionService.AsignarItemAsync(request);

        if (!resultado.Exitoso)
            return UnprocessableEntity(resultado);

        return Ok(resultado);
    }

    /// <summary>
    /// Simula la asignación de un ítem sin persistir cambios.
    /// Ideal para pruebas del algoritmo durante la evaluación técnica.
    /// </summary>
    [HttpPost("simular-asignacion")]
    [ProducesResponseType(typeof(AsignacionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SimularAsignacion([FromBody] AsignacionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resultado = await _asignacionService.SimularAsignacionAsync(request);
        return Ok(resultado);
    }
}
