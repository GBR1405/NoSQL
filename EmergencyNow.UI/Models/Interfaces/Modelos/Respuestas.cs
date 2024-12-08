using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EmergencyNow.UI.Models.Interfaces.Modelos
{
    public class Respuestas
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // _id

        [BsonRepresentation(BsonType.ObjectId)]
        public string IdIncidente { get; set; }  // ID del incidente relacionado

        public Guid IdUsuario{ get; set; }  // ID de la organización que responde

        [BsonRepresentation(BsonType.ObjectId)]
        public string IdTipoRespuesta { get; set; }  // ID del tipo de respuesta utilizada

        [BsonRequired]
        public string Estado { get; set; }  // Estado de la respuesta (por ejemplo, pendiente, completada)

        [BsonRequired]
        public DateTime HoraDeRespuesta { get; set; }  // Hora en la que se emitió la respuesta
    }
}
