using EmergencyNow.UI.AccesoADatos.Reportes.Crear;
using EmergencyNow.UI.Models.Interfaces.Reportes;

namespace EmergencyNow.UI.LN.Reportes.Crear
{
    public class CrearReporteLN
    {
        private readonly CrearReporteAD _accesoADatos;

        // Constructor con inyección de dependencias
        public CrearReporteLN(CrearReporteAD accesoADatos)
        {
            _accesoADatos = accesoADatos;
        }

        
    }
}
