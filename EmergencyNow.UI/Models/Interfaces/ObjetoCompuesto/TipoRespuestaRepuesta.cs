using EmergencyNow.UI.Models.Interfaces.Modelos;
using EmergencyNow.UI.Models.Interfaces.Reportes;

namespace EmergencyNow.UI.Models.Interfaces.ObjetoCompuesto
{
    public class TipoRespuestaRepuesta
    {
        public ReporteDto Reporte { get; set; } = new ReporteDto();
        public List<TipoDeRespuesta> Respuestas { get; set; }
    }
}
