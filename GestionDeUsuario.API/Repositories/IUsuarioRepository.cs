using GestionDeUsuario.API.Models;

namespace GestionDeUsuario.API.Repositories;

/// <summary>
/// Contrato del repositorio de usuarios — Capa de Acceso a Datos.
/// </summary>
public interface IUsuarioRepository
{
    Task<IEnumerable<Usuario>> ObtenerTodosAsync();
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<Usuario?> ObtenerPorEmailAsync(string email);
    Task<Usuario> CrearAsync(Usuario usuario);
    Task<Usuario?> ActualizarAsync(Usuario usuario);
    Task<bool> EliminarAsync(int id);
    Task<bool> ExisteEmailAsync(string email, int? excludeId = null);
}
