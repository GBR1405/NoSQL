using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace EmergencyNow.UI.Models
{
    [CollectionName("Usuarios")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {

        public string Ubicacion { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public string NombreUsuario { get; set; }


    }
}
