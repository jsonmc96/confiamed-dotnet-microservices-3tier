using ItemsDeTrabajo.API.Models;
using ItemsDeTrabajo.API.Repositories;

namespace ItemsDeTrabajo.API.Services;

/// <summary>
/// Contrato del servicio de ítems de trabajo (operaciones CRUD).
/// </summary>
public interface IItemService
{
    Task<IEnumerable<ItemTrabajo>> ObtenerTodosAsync();
    Task<ItemTrabajo?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<ItemTrabajo>> ObtenerPorUsuarioAsync(int usuarioId);
    Task<ItemTrabajo> CrearAsync(ItemTrabajo item);
    Task<ItemTrabajo?> ActualizarAsync(ItemTrabajo item);
    Task<bool> EliminarAsync(int id);
}

/// <summary>
/// Servicio de gestión CRUD de ítems de trabajo.
/// Delega el acceso a datos al repositorio correspondiente.
/// </summary>
public class ItemService : IItemService
{
    private readonly IItemRepository _repository;
    private readonly ILogger<ItemService> _logger;

    public ItemService(IItemRepository repository, ILogger<ItemService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<ItemTrabajo>> ObtenerTodosAsync()
        => await _repository.ObtenerTodosAsync();

    public async Task<ItemTrabajo?> ObtenerPorIdAsync(int id)
        => await _repository.ObtenerPorIdAsync(id);

    public async Task<IEnumerable<ItemTrabajo>> ObtenerPorUsuarioAsync(int usuarioId)
        => await _repository.ObtenerPorUsuarioAsync(usuarioId);

    public async Task<ItemTrabajo> CrearAsync(ItemTrabajo item)
    {
        var creado = await _repository.CrearAsync(item);
        _logger.LogInformation("Ítem creado con ID {Id}: {Titulo}", creado.Id, creado.Titulo);
        return creado;
    }

    public async Task<ItemTrabajo?> ActualizarAsync(ItemTrabajo item)
    {
        var actualizado = await _repository.ActualizarAsync(item);
        if (actualizado == null)
            _logger.LogWarning("Intento de actualizar ítem inexistente con ID {Id}", item.Id);
        return actualizado;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var eliminado = await _repository.EliminarAsync(id);
        if (!eliminado)
            _logger.LogWarning("Intento de eliminar ítem inexistente con ID {Id}", id);
        return eliminado;
    }
}
