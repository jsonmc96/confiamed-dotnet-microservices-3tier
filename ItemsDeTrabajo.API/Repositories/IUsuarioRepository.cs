using ItemsDeTrabajo.API.Models;

namespace ItemsDeTrabajo.API.Repositories;

/// <summary>
/// Contrato del repositorio de usuarios (local, para el contexto de asignación).
/// </summary>
public interface IUsuarioRepository
{
    Task<IEnumerable<Usuario>> ObtenerTodosConItemsAsync();
    Task<Usuario?> ObtenerPorIdConItemsAsync(int id);
}
