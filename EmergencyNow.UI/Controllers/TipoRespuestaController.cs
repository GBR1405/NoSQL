using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmergencyNow.UI.Controllers
{
    public class TipoRespuestaController : Controller
    {
        // GET: TipoRespuestaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TipoRespuestaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TipoRespuestaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoRespuestaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: TipoRespuestaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TipoRespuestaController/Edit/5
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

        // GET: TipoRespuestaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TipoRespuestaController/Delete/5
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
    }
}
