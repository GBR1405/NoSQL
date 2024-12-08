using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmergencyNow.UI.Models
{
    public class User
    {
        [Required]
        [DisplayName("Username")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Nombre")]
        public string NombreUsuario { get; set; }

        [Required]
        [EmailAddress]
        [DisplayName("Correo")]
        public string Email { get; set; }

        [Required]
        [DisplayName("Contraseña")]
        public string Password { get; set; }

        [Required]
        [DisplayName("Primer Apellido")]
        public string Apellido1 { get; set; }

        [Required]
        [DisplayName("Segundo Apellido")]
        public string Apellido2 { get; set; }

        [Required]
        public string Ubicacion { get; set; }

        [Required]
        public string Telefono { get; set; }

        public string AgregarRol { get; set; }


    }
}
