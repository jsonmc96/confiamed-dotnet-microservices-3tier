using ItemsDeTrabajo.API.Models;

namespace ItemsDeTrabajo.API.Repositories;

/// <summary>
/// Implementación en memoria del repositorio de ítems de trabajo.
/// En producción se reemplazaría por EF Core u otro ORM.
/// </summary>
public class ItemRepository : IItemRepository
{
    // Almacenamiento en memoria simulando una base de datos
    private static readonly List<ItemTrabajo> _items = new();
    private static int _nextId = 1;

    public Task<IEnumerable<ItemTrabajo>> ObtenerTodosAsync()
        => Task.FromResult<IEnumerable<ItemTrabajo>>(_items.ToList());

    public Task<ItemTrabajo?> ObtenerPorIdAsync(int id)
        => Task.FromResult(_items.FirstOrDefault(i => i.Id == id));

    public Task<IEnumerable<ItemTrabajo>> ObtenerPorUsuarioAsync(int usuarioId)
        => Task.FromResult<IEnumerable<ItemTrabajo>>(
            _items.Where(i => i.UsuarioAsignadoId == usuarioId).ToList());

    public Task<ItemTrabajo> CrearAsync(ItemTrabajo item)
    {
        item.Id = _nextId++;
        item.FechaCreacion = DateTime.UtcNow;
        _items.Add(item);
        return Task.FromResult(item);
    }

    public Task<ItemTrabajo?> ActualizarAsync(ItemTrabajo item)
    {
        var index = _items.FindIndex(i => i.Id == item.Id);
        if (index == -1) return Task.FromResult<ItemTrabajo?>(null);

        _items[index] = item;
        return Task.FromResult<ItemTrabajo?>(item);
    }

    public Task<bool> EliminarAsync(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null) return Task.FromResult(false);

        _items.Remove(item);
        return Task.FromResult(true);
    }
}
