using EmergencyNow.UI.Models.Interfaces.Modelos;

namespace EmergencyNow.UI.Abstracciones.AccesoADatos.TipoRespuesta
{
    public interface ITipoRespuestaAD
    {
        Task<List<TipoDeRespuesta>> ObtenerTiposRespuestaPorUsuario(Guid usuarioId);

        Task<bool> GuardarTipoDeRespuestaAsync(TipoDeRespuesta tipoDeRespuesta);

        Task<bool> EditarTipoDeRespuestaAsync(TipoDeRespuesta tipoDeRespuesta);

       // Task<bool> CambiarEstadoTipoDeRespuestaAsync(string tipoDeRespuestaId);

        Task<TipoDeRespuesta> ObtenerTipoRespuestaPorIdAsync(string id);

        Task<bool> ActualizarTipoRespuestaAsync(TipoDeRespuesta tipoRespuesta);

        Task<bool> CambiarOcupacion(string id, int tipo);

        Task<List<TipoDeRespuesta>> ObtenerTiposRespuestaPorOrganizacion(string organizacionId);
    }
}
