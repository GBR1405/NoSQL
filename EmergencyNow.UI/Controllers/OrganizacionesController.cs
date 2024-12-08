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
            // Obtener los reportes activos desde la capa de acceso a datos
            var reportesActivos = _crearReporte.ObtenerReportesActivos();
            return View(reportesActivos);
        }

        public async Task<IActionResult> MostrarEquipoDisponible()
        {
            // Obtener el usuario logueado
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

            return View(tiposRespuesta); // Aquí pasamos directamente los resultados a la vista
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

        // Método para formatear el string como GUID
        private string FormatGuid(string id)
        {
            // Aseguramos que el string tenga el formato adecuado con guiones
            return id.Length == 32 ?
                $"{id.Substring(0, 8)}-{id.Substring(8, 4)}-{id.Substring(12, 4)}-{id.Substring(16, 4)}-{id.Substring(20)}"
                : id; // Si ya tiene el formato, se devuelve tal cual
        }





        // GET: OrganizacionesController/Create
        public ActionResult Create()
        {
            // Aquí puedes inicializar el modelo vacío para que no sea null
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
                // Crear el usuario
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

                // Crear el usuario en la base de datos
                IdentityResult result = await _userManager.CreateAsync(appUser, usuarioOrganizacion.Usuario.Password);

                if (result.Succeeded)
                {
                    // Asignar un rol al usuario (opcional)
                    await _userManager.AddToRoleAsync(appUser, "Organizador");

                    // Obtener el ID del usuario creado
                    string userId = appUser.Id.ToString();

                    // Ahora agregamos el ID del usuario a la organización
                    Organizaciones organizacion = usuarioOrganizacion.Organizacion;
                    organizacion.UsuarioId = appUser.Id;  // Convertir userId a ObjectId

                    // Guardamos la organización en la base de datos
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
                // Crear el usuario
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

                // Crear el usuario en la base de datos
                IdentityResult result = await _userManager.CreateAsync(appUser, usuarioTipoRespuesta.Usuario.Password);

                if (result.Succeeded)
                {
                    // Asignar un rol al usuario (opcional)
                    await _userManager.AddToRoleAsync(appUser, "EquipoRescate");
                var usuario = await _userManager.GetUserAsync(User);

                // Ahora agregamos el ID del usuario a TipoDeRespuesta
                TipoDeRespuesta tipoRespuesta = usuarioTipoRespuesta.Respuestas;
                    tipoRespuesta.IdUsuario = appUser.Id;  // Asegúrate de convertir el userId a Guid si es necesario
                    tipoRespuesta.IdOrganizacion = usuario.Id;

                    // Guardar el tipo de respuesta en la base de datos usando el método AD
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

            return View(tipoDeRespuesta); // Pasamos el objeto deserializado a la vista.
        }
        

        [HttpPost]
        public async Task<IActionResult> CambiarEstadoConfirmado(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("El ID no puede estar vacío.");
            }

            // Obtener el TipoDeRespuesta por ID
            var tipoDeRespuesta = await _tipoRespuestaAD.ObtenerTipoRespuestaPorIdAsync(id);

            if (tipoDeRespuesta == null)
            {
                return NotFound("El tipo de respuesta no fue encontrado.");
            }

            // Invertir el estado actual
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

            // Guardar los cambios en la base de datos (MongoDB)
            var resultado = await _tipoRespuestaAD.ActualizarTipoRespuestaAsync(tipoDeRespuesta);

            if (resultado)
            {
                TempData["Message"] = $"El estado se cambió a '{tipoDeRespuesta.Estado}' con éxito.";
                return RedirectToAction(nameof(MostrarEquipoDisponible)); // Redirige a la vista que prefieras
            }
            else
            {
                ModelState.AddModelError("", "Hubo un error al cambiar el estado.");
                return View(tipoDeRespuesta); // Devuelve la vista con el modelo actual
            }
        }



        public async Task<IActionResult> Editar(string id)
        {
            // Obtener el TipoDeRespuesta por ID
            var tipoRespuesta = await _tipoRespuestaAD.ObtenerTipoRespuestaPorIdAsync(id);

            if (tipoRespuesta == null)
            {
                return NotFound("El tipo de respuesta no fue encontrado.");
            }

            return View(tipoRespuesta); // Pasamos el objeto a la vista
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(TipoDeRespuesta tipoRespuesta)
        {
            if (ModelState.IsValid)
            {
                // Actualizar el objeto en la base de datos
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

            return View(tipoRespuesta); // Si hay errores, volvemos a mostrar la vista con el modelo actual
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
