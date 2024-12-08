using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EmergencyNow.UI.Models.Interfaces.Reportes
{
    public class Reporte
    {
        [BsonId] 
        [BsonRepresentation(BsonType.ObjectId)] 
        public string Id { get; set; }
        public string TipoIncidente { get; set; }
        public string Descripcion { get; set; }
        public int Gravedad { get; set; }
        public Ubicacion Ubicacion { get; set; }
        public Estado Estado { get; set; }
        public ReportadoPor ReportadoPor { get; set; }

        public string IdUsuario { get; set; }
    }

    public class Ubicacion
    {
        public string Type { get; set; } = "Point";
        public double[] Coordinates { get; set; }
    }

    public class Estado
    {
        public string EstadoActual { get; set; }
        public DateTime UltimaActualizacion { get; set; }
    }

    public class ReportadoPor
    {
        public string UsuarioId { get; set; }
        public DateTime FechaReporte { get; set; }
    }
}
