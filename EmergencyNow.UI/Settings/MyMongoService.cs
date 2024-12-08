using EmergencyNow.UI.Models.Interfaces.Reportes;
using MongoDB.Driver;

namespace EmergencyNow.UI.Settings
{
    public class MyMongoService
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _database;

        public MyMongoService(IMongoClient mongoClient, MongoDbConfig mongoDbConfig)
        {
            _mongoClient = mongoClient;
            _database = _mongoClient.GetDatabase(mongoDbConfig.Name);
        }

        public IMongoCollection<ReporteDto> GetCollection()
        {
            return _database.GetCollection<ReporteDto>("Incidentes");
        }
    }

}
