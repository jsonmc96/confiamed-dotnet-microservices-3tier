using ItemsDeTrabajo.API.Models;

namespace ItemsDeTrabajo.API.Services;

/// <summary>
/// Contrato del servicio de asignación de ítems de trabajo.
/// </summary>
public interface IAsignacionService
{
    /// <summary>
    /// Ejecuta el algoritmo de asignación y asigna el ítem al usuario correspondiente.
    /// </summary>
    /// <param name="request">Datos del nuevo ítem a asignar.</param>
    /// <returns>Resultado de la asignación con el usuario elegido y la regla aplicada.</returns>
    Task<AsignacionResult> AsignarItemAsync(AsignacionRequest request);

    /// <summary>
    /// Determina a qué usuario se asignaría el ítem sin persistir el cambio.
    /// Útil para pruebas y simulaciones.
    /// </summary>
    Task<AsignacionResult> SimularAsignacionAsync(AsignacionRequest request);
}
