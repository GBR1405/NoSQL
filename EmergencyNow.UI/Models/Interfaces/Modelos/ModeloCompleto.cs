using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EmergencyNow.UI.Models.Interfaces.Modelos
{
    public class ModeloCompleto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TipoIncidente { get; set; }
        public string Descripcion { get; set; }
        public int Gravedad { get; set; }
        public double[] Ubicacion { get; set; }
        public string Estado { get; set; }

        //Modelo TipoDeRespuesta

        [BsonRequired]
        public string Placa { get; set; }  // Placa del vehículo o equipo

        [BsonRequired]
        public string TipoDeRespuestaGeneral { get; set; }  // Tipo de respuesta (médica, policial, etc.)

        [BsonRequired]
        public int CantidadMaxDePersonas { get; set; }  // Capacidad máxima de personas

        //Organizacion

        public string Nombre { get; set; }

        //Respuestas

        [BsonRequired]
        public string EstadoR { get; set; }  // Estado de la respuesta (por ejemplo, pendiente, completada)

        [BsonRequired]
        public DateTime HoraDeRespuesta { get; set; }  // Hora en la que se emitió la respuesta

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

