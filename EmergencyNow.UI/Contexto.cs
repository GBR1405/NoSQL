using EmergencyNow.UI.Models;
using EmergencyNow.UI.Models.Interfaces.Modelos;
using EmergencyNow.UI.Models.Interfaces.Reportes;
using MongoDB.Driver;

namespace EmergencyNow.UI
{
    public class Contexto
    {
        private readonly IMongoDatabase _database;

        public Contexto(IMongoClient mongoClient, string databaseName)
        {
            _database = mongoClient.GetDatabase(databaseName);
        }

        public IMongoCollection<ReporteDto> Reportes => _database.GetCollection<ReporteDto>("Incidentes");
        public IMongoCollection<Organizaciones> Organizaciones => _database.GetCollection<Organizaciones>("Organizaciones");
        public IMongoCollection<Respuestas> Respuestas => _database.GetCollection<Respuestas>("Respuestas");
        public IMongoCollection<TipoDeRespuesta> TipoRespuesta => _database.GetCollection<TipoDeRespuesta>("TipoDeRespuesta");

        // Agrega más colecciones aquí según sea necesario
    }

}
