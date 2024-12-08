using EmergencyNow.UI.Models;
using EmergencyNow.UI.Models.Interfaces.Reportes;

namespace EmergencyNow.UI.Models.Interfaces.ObjetoCompuesto
{
    public class UsuarioOrganizacion
    {
        public User Usuario { get; set; } = new User();
        public Organizaciones Organizacion { get; set; } = new Organizaciones();
    }

}
