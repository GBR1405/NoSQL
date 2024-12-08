using System.Diagnostics;
using EmergencyNow.UI.Abstracciones.AccesoADatos.Organizacion;
using EmergencyNow.UI.Models;
using EmergencyNow.UI.Models.Interfaces.Reportes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmergencyNow.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOrganizacionAD _organizacionAD;


        public HomeController(ILogger<HomeController> logger, IOrganizacionAD organizacionAD)
        {
            _logger = logger;
            _organizacionAD = organizacionAD;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Nosotros()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Sucursales()
        {
            var organizaciones = await _organizacionAD.ObtenerTodasLasSucursalesAsync();
            return View(organizaciones);
        }

        [AllowAnonymous]
        public async Task<IActionResult> BuscarOrganizacionAjax(string buscar)
        {
            var organizaciones = await _organizacionAD.BuscarPorNombreAsync(buscar);
            return PartialView("_OrganizacionesCards", organizaciones);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
