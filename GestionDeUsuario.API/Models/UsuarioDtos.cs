using System.ComponentModel.DataAnnotations;

namespace GestionDeUsuario.API.Models;

/// <summary>
/// DTO para crear un nuevo usuario.
/// </summary>
public class CreateUsuarioRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;

    public string Rol { get; set; } = "Colaborador";
}

/// <summary>
/// DTO para actualizar datos de un usuario existente.
/// </summary>
public class UpdateUsuarioRequest
{
    [MaxLength(150)]
    public string? Nombre { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public string? Rol { get; set; }
    public bool? Activo { get; set; }
}
