using EmergencyNow.UI.Abstracciones.AccesoADatos.Organizacion;
using EmergencyNow.UI.Models;
using EmergencyNow.UI.Models.Interfaces.Reportes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EmergencyNow.UI.AccesoADatos.Organizacion
{
    public class OrganizacionAD : IOrganizacionAD
    {

        private readonly IMongoCollection<Organizaciones> _Organizaciones;

        public OrganizacionAD(Contexto contexto)
        {
            _Organizaciones = contexto.Organizaciones;
        }

        public async Task<bool> AgregarOrganizacionAsync(Organizaciones organizacion)
        {
            try
            {
                await _Organizaciones.InsertOneAsync(organizacion);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Organizaciones>> ObtenerTodasLasSucursalesAsync()
        {
            try
            {
                var sucursales = await _Organizaciones.Find(_ => true).ToListAsync();
                return sucursales;
            }
            catch
            {
                return new List<Organizaciones>();
            }
        }

        public async Task<List<Organizaciones>> BuscarPorNombreAsync(string nombre)
        {
            var filtro = Builders<Organizaciones>.Filter.Regex(x => x.Nombre, new BsonRegularExpression(nombre, "i"));
            return await _Organizaciones.Find(filtro).ToListAsync();
        }


    }
}
