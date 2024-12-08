using EmergencyNow.UI.Abstracciones.AccesoADatos.Organizacion;
using EmergencyNow.UI.Abstracciones.AccesoADatos.Reportes.Crear;
using EmergencyNow.UI.Abstracciones.AccesoADatos.Respuesta;
using EmergencyNow.UI.AccesoADatos.Reportes.Crear;
using EmergencyNow.UI.LN.Reportes.Crear;
using EmergencyNow.UI.Models;
using EmergencyNow.UI.Models.Interfaces.Reportes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace EmergencyNow.UI.Controllers
{
    public class ReportesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICrearReporte _crearReporteAD;
        private readonly IRespuestasAD _Respuesta;

        // Aquí se debe recibir la interfaz (ICrearReporte) en lugar de la clase directamente
        public ReportesController(ICrearReporte crearReporteAD, UserManager<ApplicationUser> userManager, IRespuestasAD repuesta)
        {
            _crearReporteAD = crearReporteAD;
            _userManager = userManager;
            _Respuesta = repuesta;
        }

        // GET: ReportesController
        public async Task<IActionResult> Historial()
        {
            var userId = _userManager.GetUserId(User);
            var reportes = await _crearReporteAD.ObtenerReportesPorUsuarioAsync(userId);
            return View(reportes);
        }

        // GET: ReportesController/Details/5
        public async Task<IActionResult> Detalle(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var reporte = await _crearReporteAD.ObtenerReportePorIdAsync(id);
            if (reporte == null)
            {
                return NotFound();
            }

            return View(reporte);
        }


        // Mostrar vista para crear reporte
        public IActionResult Crear()
        {
            return View();
        }

        // Acción para enviar el reporte
        [HttpPost]
        public async Task<IActionResult> Crear(ReporteDto reporte)
        {
            // Verificar si la ubicación fue enviada
            if (!string.IsNullOrEmpty(reporte.UbicacionString))
            {
                // Dividir las coordenadas recibidas por la coma
                var coords = reporte.UbicacionString.Split(',');

                // Depuración: Mostrar las coordenadas recibidas
                Console.WriteLine($"Coordenadas recibidas: Lat = {coords[0]}, Lng = {coords[1]}");

                // Verificar que tenemos exactamente 2 coordenadas
                if (coords.Length == 2 &&
                    double.TryParse(coords[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(coords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double lng))
                {
                    // Asignar las coordenadas al objeto reporte
                    reporte.Ubicacion = new double[] { lat, lng };
                }
                else
                {
                    // Si no se pudieron analizar las coordenadas, agregar error al estado del modelo
                    ModelState.AddModelError("UbicacionString", "Coordenadas inválidas.");
                    return View(reporte);
                }
            }
            else
            {
                // Si no se envió ninguna ubicación, agregar error al estado del modelo
                ModelState.AddModelError("UbicacionString", "Por favor selecciona una ubicación en el mapa.");
                return View(reporte);
            }

            // Asignar el ID del usuario autenticado
            reporte.IdUsuario = _userManager.GetUserId(User);

            // Guardar el reporte (simulación)
            var resultado = await _crearReporteAD.AgregarReporteAsync(reporte);
            if (resultado)
            {
                TempData["Mensaje"] = "El reporte ha sido enviado correctamente.";
                return RedirectToAction("Index", "Home");

            }

            // Si hubo un error, agregarlo al estado del modelo
            ModelState.AddModelError("", "Hubo un error al guardar el reporte.");
            return View(reporte);
        }


        // GET: RespuestaController/Edit/5
        public async Task<IActionResult> DetalleCompleto(string id)
        {

            var DetalleCompletos = await _crearReporteAD.ObtenerReporteCompletoAsync(id);

            return View(DetalleCompletos);
        }

        public async Task<IActionResult> DetalleCompletoOrganizacion(string id)
        {

            var DetalleCompletos = await _crearReporteAD.ObtenerReporteCompletoAsync(id);

            return View(DetalleCompletos);
        }

        public async Task<IActionResult> DetalleCompletoTipoDeRespuesta(string id)
        {

            var DetalleCompletos = await _crearReporteAD.ObtenerReporteCompletoPorTipoRespuestaAsync(id);
            return View(DetalleCompletos);
        }

        public async Task<IActionResult> HistorialOrganizacion()
        {
            var userId = _userManager.GetUserId(User);
            var DetalleCompletos = await _crearReporteAD.ObtenerReportesPorUsuario(userId);

            return View(DetalleCompletos);
        }



        // GET: ReportesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReportesController/Edit/5
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

        // GET: ReportesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReportesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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

        // GET: ReportesController/Delete/5
        public async Task<ActionResult> HistorialTipoDeRespuesta()
        {
            var userIdString = _userManager.GetUserId(User);

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("El ID del usuario no es válido.");
            }

            var historial = await _Respuesta.ObtenerReportesPorUsuarioTipoRespuestaAsync(userId);

            return View(historial);
        }

    }
}
