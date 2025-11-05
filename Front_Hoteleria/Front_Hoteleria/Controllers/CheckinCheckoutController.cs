using Front_Hoteleria.Dto.Checkin;
using Front_Hoteleria.Services.Checkin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class CheckinCheckoutController : Controller
    {
        private readonly ICheckinService _api;

        public CheckinCheckoutController() : this(new CheckinService()) { }
        public CheckinCheckoutController(ICheckinService api)
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
                Trace.TraceError($"[CheckinCheckoutController.GetBearer] {ex}");
                return null;
            }
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "Checkin / Checkout";
            // puedes precargar KPIs
            var token = GetBearer();
            var kpi = await _api.KpiAsync(DateTime.Today, token) ?? new CheckinKpiDto();
            return View("~/Views/CheckinCheckout/Index.cshtml", kpi);
        }

        // tabla que se refresca por ajax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Tabla(DateTime? fecha, string estado)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

            var lista = await _api.ListarReservasAsync(fecha, estado, token)
                        ?? new List<ReservaCheckinDto>();

            return PartialView("~/Views/CheckinCheckout/_TablaCheckin.cshtml", lista);
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            return PartialView("~/Views/CheckinCheckout/_DashboardCheckin.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Checkin(CheckinAccionDto dto)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            dto.Fecha = DateTime.Now;
            var ok = await _api.HacerCheckinAsync(dto, token);
            return Json(new { ok, message = ok ? "Check-in registrado." : "No se pudo registrar el check-in." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Checkout(CheckinAccionDto dto)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            dto.Fecha = DateTime.Now;
            var ok = await _api.HacerCheckoutAsync(dto, token);
            return Json(new { ok, message = ok ? "Check-out registrado." : "No se pudo registrar el check-out." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> NoShow(CheckinAccionDto dto)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            dto.Fecha = DateTime.Now;
            var ok = await _api.RegistrarNoShowAsync(dto, token);
            return Json(new { ok, message = ok ? "No Show registrado." : "No se pudo registrar el No Show." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Extender(CheckinExtensionDto dto)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            var ok = await _api.ExtenderReservaAsync(dto, token);
            return Json(new { ok, message = ok ? "Reserva extendida." : "No se pudo extender la reserva." });
        }
    }
}
