using EmergencyNow.UI.Models.Interfaces.Modelos;
using EmergencyNow.UI.Models.Interfaces.Reportes;

namespace EmergencyNow.UI.Abstracciones.AccesoADatos.Respuesta
{
    public interface IRespuestasAD
    {
        Task<bool> AgregarReporteAsync(Respuestas Respuesta);

        Task<Respuestas> ObtenerRespuestasEnProcesoPorUsuarioAsync(Guid usuarioId);

        Task<Respuestas> ObtenerRespuestaActivaPorUsuario(Guid idUsuario);
        Task<bool> ActualizarEstadoRespuestaAsync(Guid idUsuario, string nuevoEstado);

        Task<List<ReporteDto>> ObtenerReportesPorUsuarioTipoRespuestaAsync(Guid idUsuarioTipoRespuesta);
    }
}
