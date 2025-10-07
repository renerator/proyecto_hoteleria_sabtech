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
        public ActionResult CargarMenu()
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
    }
}
