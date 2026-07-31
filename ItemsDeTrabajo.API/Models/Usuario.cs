namespace ItemsDeTrabajo.API.Models;

/// <summary>
/// Representación local de un usuario para la lógica de asignación.
/// La fuente de verdad vive en GestionDeUsuario.API.
/// </summary>
public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Ítems de trabajo asignados a este usuario.
    /// </summary>
    public List<ItemTrabajo> Items { get; set; } = new();

    /// <summary>
    /// Devuelve el número total de ítems asignados (todos los estados).
    /// </summary>
    public int TotalItems => Items.Count;

    /// <summary>
    /// Devuelve el número de ítems en estado Pendiente o EnProgreso.
    /// </summary>
    public int ItemsPendientes => Items.Count(i =>
        i.Estado == EstadoItem.Pendiente || i.Estado == EstadoItem.EnProgreso);
}
