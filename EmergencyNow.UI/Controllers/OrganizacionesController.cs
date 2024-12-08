using EmergencyNow.UI.Abstracciones.AccesoADatos.Organizacion;
using EmergencyNow.UI.Abstracciones.AccesoADatos.Reportes.Crear;
using EmergencyNow.UI.Abstracciones.AccesoADatos.TipoRespuesta;
using EmergencyNow.UI.AccesoADatos.Reportes.Crear;
using EmergencyNow.UI.AccesoADatos.TipoRespuesta;
using EmergencyNow.UI.Models;
using EmergencyNow.UI.Models.Interfaces.Modelos;
using EmergencyNow.UI.Models.Interfaces.ObjetoCompuesto;
using EmergencyNow.UI.Models.Interfaces.Reportes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace EmergencyNow.UI.Controllers
{
    public class OrganizacionesController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrganizacionAD _organizacionAD;
        private readonly ICrearReporte _crearReporte;
        private readonly ITipoRespuestaAD _tipoRespuestaAD;

        public OrganizacionesController(UserManager<ApplicationUser> userManager,
                                         IOrganizacionAD organizacionAD, ICrearReporte crearReporte, ITipoRespuestaAD tipoRespuestaAD)
        {
            _userManager = userManager;
            _organizacionAD = organizacionAD;
            _crearReporte = crearReporte;
            _tipoRespuestaAD = tipoRespuestaAD;
        }

        public IActionResult ReportesEntrantes()
        {
            var reportesActivos = _crearReporte.ObtenerReportesActivos();
            return View(reportesActivos);
        }

        public async Task<IActionResult> MostrarEquipoDisponible()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var tiposRespuesta = await _tipoRespuestaAD.ObtenerTiposRespuestaPorUsuario(usuario.Id);

            if (tiposRespuesta == null || !tiposRespuesta.Any())
            {
                ViewBag.Mensaje = "No tienes equipos de respuesta en este momento.";
            }

            return View(tiposRespuesta); 
        }

        public async Task<IActionResult> MostrarEquipoDisponiblePorId(string id)
        {

                var tiposRespuesta = await _tipoRespuestaAD.ObtenerTiposRespuestaPorOrganizacion(id);

                if (tiposRespuesta == null || !tiposRespuesta.Any())
                {
                    ViewBag.Mensaje = "No tienes equipos de respuesta en este momento.";
                    return View();
                }
                return RedirectToAction("MostrarEquipoDisponible", "Organizaciones", new { tiposRespuesta = tiposRespuesta });
        }

        private string FormatGuid(string id)
        {
            return id.Length == 32 ?
                $"{id.Substring(0, 8)}-{id.Substring(8, 4)}-{id.Substring(12, 4)}-{id.Substring(16, 4)}-{id.Substring(20)}"
                : id; 
        }





        // GET: OrganizacionesController/Create
        public ActionResult Create()
        {
            var usuarioOrganizacion = new UsuarioOrganizacion
            {
                Usuario = new User(),
                Organizacion = new Organizaciones()
            };

            return View(usuarioOrganizacion);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsuarioOrganizacion usuarioOrganizacion)
        {
            if (ModelState.IsValid)
            {
                User user = usuarioOrganizacion.Usuario;

                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.Telefono,
                    Apellido1 = user.Apellido1,
                    Apellido2 = user.Apellido2,
                    Ubicacion = user.Ubicacion,

                };

                IdentityResult result = await _userManager.CreateAsync(appUser, usuarioOrganizacion.Usuario.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, "Organizador");

                    string userId = appUser.Id.ToString();

                    Organizaciones organizacion = usuarioOrganizacion.Organizacion;
                    organizacion.UsuarioId = appUser.Id;  // Convertir userId a ObjectId

                    bool isOrganizacionSaved = await _organizacionAD.AgregarOrganizacionAsync(organizacion);

                    if (isOrganizacionSaved)
                    {
                        ViewBag.Message = "Usuario y organización creados con éxito";
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hubo un error al guardar la organización");
                    }
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(usuarioOrganizacion);
        }

        public ActionResult CrearRespuesta()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearRespuesta(UsuarioTipoRespuesta usuarioTipoRespuesta)
        {
            //if (ModelState.IsValid)
            //{
                User user = usuarioTipoRespuesta.Usuario;
                usuarioTipoRespuesta.Respuestas.Estado = "Activo";

                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.Telefono,
                    Apellido1 = user.Apellido1,
                    Apellido2 = user.Apellido2,
                    Ubicacion = user.Ubicacion,
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, usuarioTipoRespuesta.Usuario.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, "EquipoRescate");
                var usuario = await _userManager.GetUserAsync(User);

                TipoDeRespuesta tipoRespuesta = usuarioTipoRespuesta.Respuestas;
                    tipoRespuesta.IdUsuario = appUser.Id;  // Asegúrate de convertir el userId a Guid si es necesario
                    tipoRespuesta.IdOrganizacion = usuario.Id;

                    bool isTipoRespuestaSaved = await _tipoRespuestaAD.GuardarTipoDeRespuestaAsync(tipoRespuesta);

                    if (isTipoRespuestaSaved)
                    {
                        ViewBag.Message = "Usuario y tipo de respuesta creados con éxito";
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hubo un error al guardar el tipo de respuesta");
                        return View(usuarioTipoRespuesta);
                }
                }
                else
               {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                //}
            }

            return View();
        }


        // GET: OrganizacionesController/Edit/5
        public async Task<IActionResult> CambiarEstado(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("El ID no puede estar vacío.");
            }

            var tipoDeRespuesta = await _tipoRespuestaAD.ObtenerTipoRespuestaPorIdAsync(id);

            if (tipoDeRespuesta == null)
            {
                return NotFound("No se encontró el tipo de respuesta.");
            }

            return View(tipoDeRespuesta); 
        }
        

        [HttpPost]
        public async Task<IActionResult> CambiarEstadoConfirmado(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("El ID no puede estar vacío.");
            }

            var tipoDeRespuesta = await _tipoRespuestaAD.ObtenerTipoRespuestaPorIdAsync(id);

            if (tipoDeRespuesta == null)
            {
                return NotFound("El tipo de respuesta no fue encontrado.");
            }

            if (tipoDeRespuesta.Estado == "Activo")
            {
                tipoDeRespuesta.Estado = "Inactivo";
            }
            else if (tipoDeRespuesta.Estado == "Inactivo")
            {
                tipoDeRespuesta.Estado = "Activo";
            }
            else
            {
                return BadRequest("El estado actual no es válido.");
            }

            var resultado = await _tipoRespuestaAD.ActualizarTipoRespuestaAsync(tipoDeRespuesta);

            if (resultado)
            {
                TempData["Message"] = $"El estado se cambió a '{tipoDeRespuesta.Estado}' con éxito.";
                return RedirectToAction(nameof(MostrarEquipoDisponible)); 
            }
            else
            {
                ModelState.AddModelError("", "Hubo un error al cambiar el estado.");
                return View(tipoDeRespuesta); 
            }
        }



        public async Task<IActionResult> Editar(string id)
        {
            var tipoRespuesta = await _tipoRespuestaAD.ObtenerTipoRespuestaPorIdAsync(id);

            if (tipoRespuesta == null)
            {
                return NotFound("El tipo de respuesta no fue encontrado.");
            }

            return View(tipoRespuesta); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(TipoDeRespuesta tipoRespuesta)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _tipoRespuestaAD.EditarTipoDeRespuestaAsync(tipoRespuesta);

                if (resultado)
                {
                    TempData["Message"] = "Tipo de respuesta actualizado con éxito.";
                    return RedirectToAction(nameof(MostrarEquipoDisponible));
                }
                else
                {
                    ModelState.AddModelError("", "Hubo un error al actualizar el tipo de respuesta.");
                }
            }

            return View(tipoRespuesta); 
        }

        public async Task<IActionResult> Historial()
        {

            var Organizaciones = await _organizacionAD.ObtenerTodasLasSucursalesAsync();

            if (Organizaciones == null)
            {
                return NotFound("No se encontró ninguna organizaicon.");
            }

            return View(Organizaciones); 
        }

    }
}
