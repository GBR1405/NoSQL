using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EmergencyNow.UI.Models.Interfaces.Reportes
{
    public class ReporteDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] 
        public string Id { get; set; }
        public string TipoIncidente { get; set; }
        public string Descripcion { get; set; }
        public int Gravedad { get; set; }
        public double[] Ubicacion { get; set; } 
        public string UbicacionString { get; set; } 
        public string Estado { get; set; } = "Activo";
        public DateTime FechaReporte { get; set; } = DateTime.Now;
        public string IdUsuario { get; set; }
    }

}

