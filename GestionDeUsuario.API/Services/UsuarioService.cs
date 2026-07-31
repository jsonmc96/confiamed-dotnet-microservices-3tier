using GestionDeUsuario.API.Models;
using GestionDeUsuario.API.Repositories;

namespace GestionDeUsuario.API.Services;

/// <summary>
/// Contrato del servicio de gestión de usuarios — Capa de Lógica de Negocio.
/// </summary>
public interface IUsuarioService
{
    Task<IEnumerable<Usuario>> ObtenerTodosAsync();
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<(bool Exitoso, string Mensaje, Usuario? Usuario)> CrearAsync(CreateUsuarioRequest request);
    Task<(bool Exitoso, string Mensaje, Usuario? Usuario)> ActualizarAsync(int id, UpdateUsuarioRequest request);
    Task<(bool Exitoso, string Mensaje)> EliminarAsync(int id);
}

/// <summary>
/// Implementación del servicio de gestión de usuarios.
/// Aplica reglas de negocio como validación de unicidad de email.
/// </summary>
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(IUsuarioRepository repository, ILogger<UsuarioService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        => await _repository.ObtenerTodosAsync();

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
        => await _repository.ObtenerPorIdAsync(id);

    public async Task<(bool Exitoso, string Mensaje, Usuario? Usuario)> CrearAsync(
        CreateUsuarioRequest request)
    {
        // Regla de negocio: el email debe ser único
        if (await _repository.ExisteEmailAsync(request.Email))
        {
            _logger.LogWarning("Intento de crear usuario con email duplicado: {Email}", request.Email);
            return (false, $"Ya existe un usuario con el email '{request.Email}'.", null);
        }

        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Email = request.Email,
            Rol = request.Rol,
            Activo = true
        };

        var creado = await _repository.CrearAsync(usuario);
        _logger.LogInformation("Usuario creado con ID {Id}: {Nombre}", creado.Id, creado.Nombre);

        return (true, "Usuario creado exitosamente.", creado);
    }

    public async Task<(bool Exitoso, string Mensaje, Usuario? Usuario)> ActualizarAsync(
        int id, UpdateUsuarioRequest request)
    {
        var usuario = await _repository.ObtenerPorIdAsync(id);
        if (usuario == null)
            return (false, $"Usuario con ID {id} no encontrado.", null);

        // Validar unicidad de email si se está cambiando
        if (request.Email != null && request.Email != usuario.Email)
        {
            if (await _repository.ExisteEmailAsync(request.Email, excludeId: id))
                return (false, $"Ya existe un usuario con el email '{request.Email}'.", null);
        }

        // Actualizar sólo los campos proporcionados (patrón PATCH-like)
        if (request.Nombre != null) usuario.Nombre = request.Nombre;
        if (request.Email != null) usuario.Email = request.Email;
        if (request.Rol != null) usuario.Rol = request.Rol;
        if (request.Activo.HasValue) usuario.Activo = request.Activo.Value;

        var actualizado = await _repository.ActualizarAsync(usuario);
        return (true, "Usuario actualizado exitosamente.", actualizado);
    }

    public async Task<(bool Exitoso, string Mensaje)> EliminarAsync(int id)
    {
        var eliminado = await _repository.EliminarAsync(id);
        if (!eliminado)
            return (false, $"Usuario con ID {id} no encontrado.");

        _logger.LogInformation("Usuario con ID {Id} eliminado.", id);
        return (true, "Usuario eliminado exitosamente.");
    }
}
