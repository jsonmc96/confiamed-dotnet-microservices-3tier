namespace GestionDeUsuario.API.Models;

/// <summary>
/// Entidad principal del usuario en el microservicio de gestión.
/// </summary>
public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = "Colaborador";
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Resumen de carga de trabajo del usuario (datos integrados desde ItemsDeTrabajo.API).
    /// </summary>
    public ResumenCargaTrabajo? Carga { get; set; }
}

/// <summary>
/// Resumen de la carga de trabajo de un usuario, calculado a partir
/// de los datos del microservicio de ítems.
/// </summary>
public class ResumenCargaTrabajo
{
    public int TotalItems { get; set; }
    public int ItemsPendientes { get; set; }
    public int ItemsRelevantes { get; set; }
    public int ItemsCompletados { get; set; }
}
