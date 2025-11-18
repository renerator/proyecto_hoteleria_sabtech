using Front_Hoteleria.Dto.SolicitudServicio;
using Front_Hoteleria.Services.SolicitudServicio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class SolicitudServicioController : Controller
    {
        private readonly ISolicitudServicioService _api;

        public SolicitudServicioController() : this(new SolicitudServicioService()) { }

        public SolicitudServicioController(ISolicitudServicioService api)
        {
            _api = api;
        }

        private string GetBearer()
        {
            try
            {
                return (Session["Token"] as string)
                       ?? (Request.Cookies["access_token"] != null
                           ? Request.Cookies["access_token"].Value
                           : null);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[GetBearer] Error leyendo token: {ex}");
                return null;
            }
        }

        // ===================== INDEX =====================
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Solicitudes de Servicios";
            return View("~/Views/SolicitudServicio/Index.cshtml");
        }

        // ===================== TABLA (AJAX) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Tabla(int idEstado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

            // idEstado = idEstadoSolicitud (1 Pendiente, 2 Asignada, 3 Rechazada, 4 Realizada, etc.)
            var lista = await _api.ListarSolicitudesVigentesAsync(fechaInicio, fechaFin, idEstado, token)
                        ?? new List<SolicitudServicioDto>();

            return PartialView("~/Views/SolicitudServicio/_TablaServicio.cshtml", lista);
        }



        [HttpGet]
        public ActionResult Paneles()
        {
            return PartialView("~/Views/SolicitudServicio/_PanelesServicio.cshtml");
        }

        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

            var kpi = await _api.ObtenerKpiAsync(token);
            return PartialView("~/Views/SolicitudServicio/_DashboardServicio.cshtml", kpi);
        }


        // ===================== UPSERT =====================
        [HttpGet]
        public async Task<ActionResult> Upsert(int? id)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

            SolicitudServicioDto model;

            if (id.HasValue && id.Value > 0)
            {
                model = await _api.ObtenerSolicitudAsync(id.Value, token)
                        ?? new SolicitudServicioDto { IdSolicitud = 0 };
            }
            else
            {
                model = new SolicitudServicioDto
                {
                    IdSolicitud = 0,
                    FechaSolicitud = DateTime.Now,
                    IdEstadoSolicitud = 1 // Pendiente
                };
            }

            return PartialView("~/Views/SolicitudServicio/_UpsertServicio.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Crear(SolicitudServicioDto dto)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            var ok = await _api.CrearSolicitudAsync(dto, token);
            return Json(new { ok, message = ok ? "Solicitud creada." : "No se pudo crear la solicitud." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Modificar(SolicitudServicioDto dto)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            var ok = await _api.ModificarSolicitudAsync(dto, token);
            return Json(new { ok, message = ok ? "Solicitud actualizada." : "No se pudo actualizar la solicitud." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Eliminar(int idSolicitud)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            var ok = await _api.EliminarSolicitudAsync(idSolicitud, token);
            return Json(new { ok, message = ok ? "Solicitud eliminada." : "No se pudo eliminar la solicitud." });
        }
    }
}
