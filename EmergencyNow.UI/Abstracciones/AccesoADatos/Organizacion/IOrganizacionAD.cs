

using EmergencyNow.UI.Models.Interfaces.Reportes;

namespace EmergencyNow.UI.Abstracciones.AccesoADatos.Organizacion
{
    public interface IOrganizacionAD
    {
        Task<bool> AgregarOrganizacionAsync(Organizaciones organizacion);

        Task<List<Organizaciones>> ObtenerTodasLasSucursalesAsync();

        Task<List<Organizaciones>> BuscarPorNombreAsync(string nombre);
    }
}
