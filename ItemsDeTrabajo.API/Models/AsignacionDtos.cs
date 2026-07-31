namespace ItemsDeTrabajo.API.Models;

/// <summary>
/// DTO de entrada para solicitar la asignación de un nuevo ítem de trabajo.
/// </summary>
public class AsignacionRequest
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool EsRelevante { get; set; }

    /// <summary>
    /// Fecha límite de entrega del ítem a asignar.
    /// </summary>
    public DateTime FechaEntrega { get; set; }
}

/// <summary>
/// DTO de respuesta tras ejecutar el algoritmo de asignación.
/// </summary>
public class AsignacionResult
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>
    /// ID del usuario al que se asignó el ítem.
    /// </summary>
    public int UsuarioAsignadoId { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Regla de negocio que determinó la asignación.
    /// </summary>
    public string ReglaAplicada { get; set; } = string.Empty;

    public ItemTrabajo? ItemCreado { get; set; }
}
