using ItemsDeTrabajo.API.Models;

namespace ItemsDeTrabajo.API.Repositories;

/// <summary>
/// Contrato del repositorio de ítems de trabajo (Capa de Acceso a Datos).
/// </summary>
public interface IItemRepository
{
    Task<IEnumerable<ItemTrabajo>> ObtenerTodosAsync();
    Task<ItemTrabajo?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<ItemTrabajo>> ObtenerPorUsuarioAsync(int usuarioId);
    Task<ItemTrabajo> CrearAsync(ItemTrabajo item);
    Task<ItemTrabajo?> ActualizarAsync(ItemTrabajo item);
    Task<bool> EliminarAsync(int id);
}
