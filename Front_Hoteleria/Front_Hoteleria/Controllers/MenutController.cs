using Front_Hoteleria.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace Front_Hoteleria.Controllers
{
    public class MenuController : Controller
    {
        private readonly MenuService _service = new MenuService();

        [ChildActionOnly] // opcional pero recomendado para evitar acceso directo

        [HttpGet]
        public ActionResult Index()
        {
            if (!(Session["Token"] is string tok) || string.IsNullOrWhiteSpace(tok))
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            return View();
        }
        public ActionResult CargarMenu()
        {
            try
            {
                var idUsuario = (int)(Session["IdUsuario"] ?? 0);
                var idPerfil = (int)(Session["IdPerfil"] ?? 0);


                // BLOQUEAR de forma segura la llamada async
                var modelo = _service.ObtenerMenuAsync(idUsuario, idPerfil)
                                     .ConfigureAwait(false)
                                     .GetAwaiter()
                                     .GetResult();

                return PartialView("_MenuPartial", modelo);
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }
    }
}
