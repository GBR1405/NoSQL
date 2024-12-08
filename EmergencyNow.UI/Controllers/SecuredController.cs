using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmergencyNow.UI.Controllers
{
    public class SecuredController : Controller
    {

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
