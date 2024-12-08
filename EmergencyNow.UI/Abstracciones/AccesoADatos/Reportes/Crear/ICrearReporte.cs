using EmergencyNow.UI.Models.Interfaces.Modelos;
using EmergencyNow.UI.Models.Interfaces.Reportes;
using MongoDB.Driver;

namespace EmergencyNow.UI.Abstracciones.AccesoADatos.Reportes.Crear
{
    public interface ICrearReporte
    {
        Task<List<ReporteDto>> ObtenerReportesPorUsuarioAsync(string idUsuario);
        Task<bool> AgregarReporteAsync(ReporteDto reporte);

        Task<ReporteDto> ObtenerReportePorIdAsync(string id);

        List<ReporteDto> ObtenerReportesActivos();

        Task<bool> ActualizarEstadoAsync(string id, int tipo);

        Task<ModeloCompleto> ObtenerReporteCompletoAsync(string id);

        Task<List<ReporteDto>> ObtenerReportesPorUsuario(string idUsuario);

        Task<ModeloCompleto> ReporteActivoAsync(Guid idUsuario);

        Task<ModeloCompleto> ObtenerReporteCompletoPorTipoRespuestaAsync(string tipoRespuestaId);
    }
}
