using Front_Hoteleria.Dto.ServiciosPersonal;
using Front_Hoteleria.Services.ServiciosPersonal;
using System;
using System.Diagnostics;
using System.Net;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class ServiciosPersonalController : Controller
    {
        private readonly IServiciosPersonalService _api;

        public ServiciosPersonalController() : this(new ServiciosPersonalService()) { }

        public ServiciosPersonalController(IServiciosPersonalService api)
        {
            _api = api;
        }

        private string GetBearer()
        {
            try
            {
                return (Session["Token"] as string)
                       ?? (Request.Cookies["access_token"] != null ? Request.Cookies["access_token"].Value : null);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ServiciosPersonalController.GetBearer] {ex}");
                return null;
            }
        }

        // ================== INDEX ==================
        [HttpGet]
        public ActionResult Index()
        {
            // mismo estilo que tu PanelPrincipal
            var tok = Session["Token"] as string;
            if (string.IsNullOrWhiteSpace(tok))
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            if (Session["IdPerfil"] == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            return View("~/Views/ServiciosPersonal/Index.cshtml");
        }

        // =================================================
        // PARCIALES QUE SE RENDERIZAN CON Html.Action(...)
        // TIENEN QUE SER SIN async
        // =================================================

        // KPI superior
        [HttpGet]
        public ActionResult _DashboardServicioPersonal()
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada");

                // llamamos al servicio de forma bloqueante para que el child action no sea async
                var task = _api.ObtenerKpiAsync(token);
                var kpi = task != null ? task.GetAwaiter().GetResult() : null;

                if (kpi == null)
                    kpi = new ServiciosPersonalKpiDto();

                return PartialView("~/Views/ServiciosPersonal/_DashboardServicioPersonal.cshtml", kpi);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[_DashboardServicioPersonal] {ex}");
                // devolvemos dto vacío para que la vista no rompa
                return PartialView("~/Views/ServiciosPersonal/_DashboardServicioPersonal.cshtml",
                    new ServiciosPersonalKpiDto());
            }
        }

        // tabla de solicitudes
        [HttpGet]
        public ActionResult _TablaServicioPersonal()
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada");

                var task = _api.ListarSolicitudesAsync(bearer: token);
                var lista = task != null ? task.GetAwaiter().GetResult() : null;

                return PartialView("~/Views/ServiciosPersonal/_TablaServicioPersonal.cshtml",
                    lista ?? new System.Collections.Generic.List<ServiciosPersonalDto>());
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[_TablaServicioPersonal] {ex}");
                return PartialView("~/Views/ServiciosPersonal/_TablaServicioPersonal.cshtml",
                    new System.Collections.Generic.List<ServiciosPersonalDto>());
            }
        }

        // paneles de abajo (activos + próximas)
        [HttpGet]
        public ActionResult _PanelesServicioPersonal()
        {
            // si después querés traer los activos por ajax, este queda así de simple
            return PartialView("~/Views/ServiciosPersonal/_PanelesServicioPersonal.cshtml");
        }

        // =================================================
        //  ACCIONES AJAX (estas sí async)
        //  las llamás desde JS para refrescar filtros, asignar, etc.
        // =================================================

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> ListarSolicitudes(
            string ordenarPor,
            string prioridad,
            string estado,
            string ubicacion)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { ok = false, message = "Sesión expirada" });

                var data = await _api.ListarSolicitudesAsync(ordenarPor, prioridad, estado, ubicacion, token);
                return Json(new { ok = true, data });
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ListarSolicitudes] {ex}");
                return Json(new { ok = false, message = "Error al listar solicitudes" });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> ListarActivos()
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { ok = false, message = "Sesión expirada" });

                var data = await _api.ListarServiciosActivosAsync(token);
                return Json(new { ok = true, data });
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ListarActivos] {ex}");
                return Json(new { ok = false, message = "Error al listar servicios activos" });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> ListarProximos()
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { ok = false, message = "Sesión expirada" });

                var data = await _api.ListarProximasSolicitudesAsync(token);
                return Json(new { ok = true, data });
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ListarProximos] {ex}");
                return Json(new { ok = false, message = "Error al listar próximas solicitudes" });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Asignar(string id)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { ok = false, message = "Sesión expirada" });

                var ok = await _api.AsignarSolicitudAsync(id, token);
                return Json(new { ok });
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Asignar] {ex}");
                return Json(new { ok = false, message = "Error al asignar solicitud" });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Iniciar(string id, string tiempoEstimado, string observaciones)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { ok = false, message = "Sesión expirada" });

                var ok = await _api.IniciarSolicitudAsync(id, tiempoEstimado, observaciones, token);
                return Json(new { ok });
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Iniciar] {ex}");
                return Json(new { ok = false, message = "Error al iniciar solicitud" });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Completar(string id, string descripcion)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { ok = false, message = "Sesión expirada" });

                var ok = await _api.CompletarServicioAsync(id, descripcion, token);
                return Json(new { ok });
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Completar] {ex}");
                return Json(new { ok = false, message = "Error al completar servicio" });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Notificar(string id, string metodo, string destino, string mensaje)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { ok = false, message = "Sesión expirada" });

                var ok = await _api.NotificarHuespedAsync(id, metodo, destino, mensaje, token);
                return Json(new { ok });
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Notificar] {ex}");
                return Json(new { ok = false, message = "Error al notificar al huésped" });
            }
        }
    }
}
