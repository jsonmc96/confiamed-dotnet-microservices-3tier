namespace ItemsDeTrabajo.API.Models;

/// <summary>
/// Representa un ítem de trabajo dentro del sistema.
/// </summary>
public class ItemTrabajo
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el ítem es altamente relevante para el negocio.
    /// </summary>
    public bool EsRelevante { get; set; }

    /// <summary>
    /// Fecha límite de entrega del ítem.
    /// </summary>
    public DateTime FechaEntrega { get; set; }

    public EstadoItem Estado { get; set; } = EstadoItem.Pendiente;

    /// <summary>
    /// ID del usuario al que está asignado (null = sin asignar).
    /// </summary>
    public int? UsuarioAsignadoId { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}

public enum EstadoItem
{
    Pendiente,
    EnProgreso,
    Completado,
    Cancelado
}
