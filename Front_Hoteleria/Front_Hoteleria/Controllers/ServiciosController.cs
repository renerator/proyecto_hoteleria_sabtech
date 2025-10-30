// Front_Hoteleria/Controllers/ServiciosController.cs
using Front_Hoteleria.Dto.Servicio;
using Front_Hoteleria.Services.Servicio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly IServicioService _api;

        public ServiciosController() : this(new ServicioService()) { }
        public ServiciosController(IServicioService api) { _api = api; }

        private string GetBearer()
        {
            try
            {
                return (Session["Token"] as string)
                       ?? (Request.Cookies["access_token"] != null ? Request.Cookies["access_token"].Value : null);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[GetBearer] Error leyendo token: {ex}");
                return null;
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Servicios";
            return View("~/Views/Servicios/Index.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Tabla(int? estado)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

            var lista = await _api.ListarServiciosAsync(estado, token) ?? new List<ServicioDto>();
            return PartialView("~/Views/Servicios/_TablaServicio.cshtml", lista);
        }

        [HttpGet]
        public ActionResult Paneles()
        {
            return PartialView("~/Views/Servicios/_PanelesServicio.cshtml");
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            return PartialView("~/Views/Servicios/_DashboardServicio.cshtml");
        }

        [HttpGet]
        public ActionResult Upsert(int? id)
        {
            // Si necesitaras precargar por ID, podríamos agregar un método GetById en el ApiClient.
            var model = new ServicioDto { IdServicio = id ?? 0, Estado = true };
            return PartialView("~/Views/Servicios/_UpsertServicio.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Crear(ServicioDto dto)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            var ok = await _api.CrearServicioAsync(dto, token);
            return Json(new { ok, message = ok ? "Servicio creado." : "No se pudo crear." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Modificar(ServicioDto dto)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            var ok = await _api.ModificarServicioAsync(dto, token);
            return Json(new { ok, message = ok ? "Servicio actualizado." : "No se pudo actualizar." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Eliminar(int idServicio)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            var ok = await _api.EliminarServicioAsync(idServicio, token);
            return Json(new { ok, message = ok ? "Servicio eliminado." : "No se pudo eliminar." });
        }
    }
}
