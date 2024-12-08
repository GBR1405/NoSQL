using Amazon.Runtime.Internal;
using EmergencyNow.UI.Abstracciones.AccesoADatos.Organizacion;
using EmergencyNow.UI.Abstracciones.AccesoADatos.Reportes.Crear;
using EmergencyNow.UI.Abstracciones.AccesoADatos.Respuesta;
using EmergencyNow.UI.Abstracciones.AccesoADatos.TipoRespuesta;
using EmergencyNow.UI.AccesoADatos.Reportes.Crear;
using EmergencyNow.UI.AccesoADatos.Respuesta;
using EmergencyNow.UI.AccesoADatos.TipoRespuesta;
using EmergencyNow.UI.Models;
using EmergencyNow.UI.Models.Interfaces.Modelos;
using EmergencyNow.UI.Models.Interfaces.ObjetoCompuesto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmergencyNow.UI.Controllers
{
    public class RespuestaController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICrearReporte _crearReporte;
        private readonly ITipoRespuestaAD _tipoRespuestaAD;
        private readonly IRespuestasAD _Respuesta;

        public RespuestaController(UserManager<ApplicationUser> userManager,
                                          ICrearReporte crearReporte, ITipoRespuestaAD tipoRespuestaAD, IRespuestasAD Respuesta)
        {
            _userManager = userManager;
            _crearReporte = crearReporte;
            _tipoRespuestaAD = tipoRespuestaAD;
            _Respuesta = Respuesta;
        }

        // GET: RespuestaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: RespuestaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RespuestaController/Create
        public async Task<IActionResult> Create(string id)
        {
            var usuario = await _userManager.GetUserAsync(User);

            // Obtener los tipos de respuesta para el usuario
            var tiposRespuesta = await _tipoRespuestaAD.ObtenerTiposRespuestaPorUsuario(usuario.Id);

            // Verificar si el usuario tiene tipos de respuesta
            if (tiposRespuesta == null || !tiposRespuesta.Any())
            {
                ViewBag.Mensaje = "No tienes equipos de respuesta en este momento.";
            }
            else
            {
                // Filtrar los tipos de respuesta activos
                var tiposRespuestaActivos = tiposRespuesta.Where(tr => tr.Estado == "Activo").ToList();

                // Si no hay tipos de respuesta activos
                if (!tiposRespuestaActivos.Any())
                {
                    ViewBag.Mensaje = "No hay equipos de respuesta activos en este momento.";
                }

                // Obtener el reporte
                var reporte = await _crearReporte.ObtenerReportePorIdAsync(id);

                // Crear el modelo para la vista
                var Eleccion = new TipoRespuestaRepuesta
                {
                    Reporte = reporte,
                    Respuestas = tiposRespuestaActivos
                };

                return View(Eleccion);
            }

            // En caso de que no haya equipos de respuesta
            return View();
        }


        // POST: RespuestaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string id, string id2)
        {
            try
            {

                var usuario = await _userManager.GetUserAsync(User);

                var RespuestaNueva = new Respuestas
                {
                    IdIncidente = id,
                    IdUsuario = usuario.Id,
                    IdTipoRespuesta = id2,
                    Estado = "En proceso",
                    HoraDeRespuesta = DateTime.Now,
                };

                await _tipoRespuestaAD.CambiarOcupacion(id2, 1);
                await _crearReporte.ActualizarEstadoAsync(id, 1);

                await _Respuesta.AgregarReporteAsync(RespuestaNueva);

                return RedirectToAction("ReportesEntrantes", "Organizaciones");

            }
            catch
            {
                return View();
            }
        }

        // GET: RespuestaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        

        // POST: RespuestaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RespuestaController/Delete/5
        public async Task<IActionResult> VerReporteActual()
        {
            // Obtener el ID del usuario logueado desde el contexto
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var guidUsuario))
            {
                return BadRequest("El ID del usuario no es válido.");
            }

            // Llamar al método del AD para obtener la respuesta activa
            var respuestaActiva = await _crearReporte.ReporteActivoAsync(guidUsuario);

            if (respuestaActiva == null)
            {
                return View(respuestaActiva); ;
            }

            // Retornar la respuesta activa
            return View(respuestaActiva);
        }

        [HttpPost]
        public async Task<IActionResult> GenerarReporte(string EstadoR)
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);
                var Reporte = await _Respuesta.ActualizarEstadoRespuestaAsync(usuario.Id, EstadoR);

                if (Reporte)
                {
                    TempData["Mensaje"] = "El reporte ha sido enviado correctamente.";
                    return RedirectToAction("VerReporteActual", "Respuesta");
                }else
                {
                    TempData["Mensaje"] = "El reporte no se pudo actualizar a causa de un error";
                    return RedirectToAction("VerReporteActual", "Respuesta");
                }
            }
            catch
            {
                return RedirectToAction("VerReporteActual", "Respuesta");
            }
        }

    }
}
