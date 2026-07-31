using ItemsDeTrabajo.API.Models;

namespace ItemsDeTrabajo.API.Repositories;

/// <summary>
/// Implementación en memoria del repositorio de usuarios para el contexto
/// de asignación de ítems. Datos iniciales representan el caso de prueba
/// del documento: Usuario A (3 ítems, 2 relevantes) y Usuario B (1 ítem, 0 relevantes).
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private static readonly List<Usuario> _usuarios;

    static UsuarioRepository()
    {
        // --- Datos de prueba del documento CONFIAMED ---
        // Usuario A: 3 ítems asignados, 2 de ellos relevantes
        var usuarioA = new Usuario { Id = 1, Nombre = "Usuario A", Email = "usuarioa@confiamed.com" };
        usuarioA.Items.AddRange(new[]
        {
            new ItemTrabajo
            {
                Id = 1, Titulo = "Ítem A1", EsRelevante = true,
                Estado = EstadoItem.Pendiente, FechaEntrega = DateTime.UtcNow.AddDays(10),
                UsuarioAsignadoId = 1
            },
            new ItemTrabajo
            {
                Id = 2, Titulo = "Ítem A2", EsRelevante = true,
                Estado = EstadoItem.EnProgreso, FechaEntrega = DateTime.UtcNow.AddDays(5),
                UsuarioAsignadoId = 1
            },
            new ItemTrabajo
            {
                Id = 3, Titulo = "Ítem A3", EsRelevante = false,
                Estado = EstadoItem.Pendiente, FechaEntrega = DateTime.UtcNow.AddDays(7),
                UsuarioAsignadoId = 1
            }
        });

        // Usuario B: 1 ítem asignado, no relevante
        var usuarioB = new Usuario { Id = 2, Nombre = "Usuario B", Email = "usuariob@confiamed.com" };
        usuarioB.Items.Add(new ItemTrabajo
        {
            Id = 4, Titulo = "Ítem B1", EsRelevante = false,
            Estado = EstadoItem.Pendiente, FechaEntrega = DateTime.UtcNow.AddDays(15),
            UsuarioAsignadoId = 2
        });

        _usuarios = new List<Usuario> { usuarioA, usuarioB };
    }

    public Task<IEnumerable<Usuario>> ObtenerTodosConItemsAsync()
        => Task.FromResult<IEnumerable<Usuario>>(_usuarios.ToList());

    public Task<Usuario?> ObtenerPorIdConItemsAsync(int id)
        => Task.FromResult(_usuarios.FirstOrDefault(u => u.Id == id));
}
