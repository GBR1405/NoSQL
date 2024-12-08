using EmergencyNow.UI.Models.Interfaces.Modelos;
using EmergencyNow.UI.Models.Interfaces.Reportes;

namespace EmergencyNow.UI.Models.Interfaces.ObjetoCompuesto
{
    public class UsuarioTipoRespuesta
    {

        public User Usuario { get; set; } = new User();
        public TipoDeRespuesta Respuestas { get; set; } = new TipoDeRespuesta();

    }
}
