using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EmergencyNow.UI.Models.Interfaces.Modelos
{
    public class TipoDeRespuesta
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // _id

        [BsonRequired]
        public string Placa { get; set; }  // Placa del vehículo o equipo

        [BsonRequired]
        public string TipoDeRespuestaGeneral { get; set; }  // Tipo de respuesta (médica, policial, etc.)

        public Guid IdUsuario { get; set; }  // ID del usuario responsable

        public Guid IdOrganizacion { get; set; }  // ID de la organización a la que pertenece

        [BsonRequired]
        public string Estado { get; set; }  // Estado (por ejemplo, disponible, en uso)

        [BsonRequired]
        public int CantidadMaxDePersonas { get; set; }  // Capacidad máxima de personas
    }
}
