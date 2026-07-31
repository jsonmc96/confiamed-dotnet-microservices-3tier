using GestionDeUsuario.API.Models;
using GestionDeUsuario.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionDeUsuario.API.Controllers;

/// <summary>
/// Controlador de gestión de usuarios — Capa de Presentación (API).
/// Expone los endpoints REST para el CRUD de usuarios.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(IUsuarioService usuarioService, ILogger<UsuariosController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la lista completa de usuarios registrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Usuario>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodos()
    {
        var usuarios = await _usuarioService.ObtenerTodosAsync();
        return Ok(usuarios);
    }

    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var usuario = await _usuarioService.ObtenerPorIdAsync(id);
        if (usuario == null)
            return NotFound(new { Mensaje = $"Usuario con ID {id} no encontrado." });

        return Ok(usuario);
    }

    /// <summary>
    /// Crea un nuevo usuario en el sistema.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Usuario), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Crear([FromBody] CreateUsuarioRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (exitoso, mensaje, usuario) = await _usuarioService.CrearAsync(request);

        if (!exitoso)
            return Conflict(new { Mensaje = mensaje });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = usuario!.Id }, usuario);
    }

    /// <summary>
    /// Actualiza los datos de un usuario existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] UpdateUsuarioRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (exitoso, mensaje, usuario) = await _usuarioService.ActualizarAsync(id, request);

        if (!exitoso && usuario == null)
        {
            // Distinguir entre "no encontrado" y "conflicto de email"
            return mensaje.Contains("encontrado")
                ? NotFound(new { Mensaje = mensaje })
                : Conflict(new { Mensaje = mensaje });
        }

        return Ok(usuario);
    }

    /// <summary>
    /// Elimina un usuario del sistema por su ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var (exitoso, mensaje) = await _usuarioService.EliminarAsync(id);

        if (!exitoso)
            return NotFound(new { Mensaje = mensaje });

        return NoContent();
    }
}
