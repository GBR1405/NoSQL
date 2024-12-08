using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EmergencyNow.UI.Models.Interfaces.Reportes
{
    public class Organizaciones
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public Guid UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }

        public Contacto Contacto { get; set; }
        public string Ubicacion { get; set; }
    }

    public class Contacto
    {
        public string Telefono { get; set; }
        public string Email { get; set; }
    }
}
