using GestionDeUsuario.API.Models;

namespace GestionDeUsuario.API.Repositories;

/// <summary>
/// Implementación en memoria del repositorio de usuarios.
/// Incluye datos de prueba que coinciden con el caso de prueba del documento CONFIAMED.
/// En producción, este repositorio sería reemplazado por una implementación con EF Core.
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private static readonly List<Usuario> _usuarios = new()
    {
        new Usuario
        {
            Id = 1,
            Nombre = "Usuario A",
            Email = "usuarioa@confiamed.com",
            Rol = "Colaborador",
            Activo = true,
            FechaCreacion = DateTime.UtcNow.AddDays(-30),
            Carga = new ResumenCargaTrabajo
            {
                TotalItems = 3,
                ItemsPendientes = 2,
                ItemsRelevantes = 2,
                ItemsCompletados = 1
            }
        },
        new Usuario
        {
            Id = 2,
            Nombre = "Usuario B",
            Email = "usuariob@confiamed.com",
            Rol = "Colaborador",
            Activo = true,
            FechaCreacion = DateTime.UtcNow.AddDays(-15),
            Carga = new ResumenCargaTrabajo
            {
                TotalItems = 1,
                ItemsPendientes = 1,
                ItemsRelevantes = 0,
                ItemsCompletados = 0
            }
        }
    };

    private static int _nextId = 3;

    public Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        => Task.FromResult<IEnumerable<Usuario>>(_usuarios.ToList());

    public Task<Usuario?> ObtenerPorIdAsync(int id)
        => Task.FromResult(_usuarios.FirstOrDefault(u => u.Id == id));

    public Task<Usuario?> ObtenerPorEmailAsync(string email)
        => Task.FromResult(_usuarios.FirstOrDefault(
            u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));

    public Task<Usuario> CrearAsync(Usuario usuario)
    {
        usuario.Id = _nextId++;
        usuario.FechaCreacion = DateTime.UtcNow;
        _usuarios.Add(usuario);
        return Task.FromResult(usuario);
    }

    public Task<Usuario?> ActualizarAsync(Usuario usuario)
    {
        var index = _usuarios.FindIndex(u => u.Id == usuario.Id);
        if (index == -1) return Task.FromResult<Usuario?>(null);

        _usuarios[index] = usuario;
        return Task.FromResult<Usuario?>(usuario);
    }

    public Task<bool> EliminarAsync(int id)
    {
        var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
        if (usuario == null) return Task.FromResult(false);

        _usuarios.Remove(usuario);
        return Task.FromResult(true);
    }

    public Task<bool> ExisteEmailAsync(string email, int? excludeId = null)
    {
        var existe = _usuarios.Any(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
            (excludeId == null || u.Id != excludeId));
        return Task.FromResult(existe);
    }
}
