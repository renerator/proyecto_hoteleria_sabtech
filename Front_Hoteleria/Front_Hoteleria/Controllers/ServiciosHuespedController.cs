using Front_Hoteleria.Dto.Huesped;
using Front_Hoteleria.Dto.Servicio;
using Front_Hoteleria.Dto.SolicitudServicio;
using Front_Hoteleria.Services.ServiciosHuesped;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class ServiciosHuespedController : Controller
    {
        private readonly IServiciosHuespedService _api;

        public ServiciosHuespedController() : this(new ServicioHuespedService()) { }

        public ServiciosHuespedController(IServiciosHuespedService api)
        {
            _api = api;
        }

        // ================= TOKEN =================
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

        // ================== GUARDAR SERVICIO ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GuardarServicio(ServicioHuespedDto dto)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { ok = false, message = "Sesión expirada." });

                var ok = await _api.CrearServicioHuespedAsync(dto, token);

                return Json(new
                {
                    ok,
                    message = ok ? "Solicitud de servicio enviada." :
                                   "No se pudo guardar la solicitud de servicio."
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ServiciosHuesped.GuardarServicio] " + ex);
                return Json(new { ok = false, message = "Error al guardar la solicitud de servicio." });
            }
        }

        // Alias para compatibilidad con código antiguo que llamaba a "Crear"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> Crear(ServicioHuespedDto dto)
            => GuardarServicio(dto);



        // ================= INDEX =================
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var tok = Session["Token"] as string;
            if (string.IsNullOrWhiteSpace(tok))
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            try
            {
                int idPerfil = 0;
                var rawPerfil = Session["IdPerfil"];
                if (rawPerfil == null || !int.TryParse(rawPerfil.ToString(), out idPerfil))
                    return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

                switch (idPerfil)
                {
                    case 1: // Administrador
                        return View("~/Views/Servicios/Index.cshtml");

                    case 2: // Huésped
                        {
                            // Llamamos al servicio que lista las solicitudes (el mismo que usa la tabla)
                            var filtro = new ServicioHuespedDto(); // filtros vacíos => todas las del huésped
                            var lista = await _api.ListarServiciosHuespedAsync(filtro, tok)
                                       ?? new List<ServicioHuespedDto>();

                            // KPIs
                            ViewBag.TotalSolicitudes = lista.Count;

                            // 1 = Pendiente, 2 = Asignada, 3 = Rechazada, 4 = Realizada
                            ViewBag.SolicitudesCompletadas = lista.Count(x => x.IdEstado == 4);
                            ViewBag.SolicitudesPendientes = lista.Count(x => x.IdEstado == 1);

                            return View("~/Views/Huesped/Servicio/Index.cshtml");
                        }

                    case 3: // Personal
                        return View("~/Views/Servicios/Gestionar.cshtml");

                    default:
                        return new HttpStatusCodeResult(403);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ServiciosHuespedController.Index] {ex}");
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
            }
        }

            // ================= PARCIALES =================

            // Paneles intermedios (si los usas en otra vista)
            [HttpGet]
        public ActionResult PanelesServicio()
            => PartialView("_PanelesServicio");

        // Tabla (listado de servicios solicitados por el huésped)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TablaServicio(int? vigencia, string nombre)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada");

                var filtro = new ServicioHuespedDto
                {
                    FiltroNombreServicio = nombre
                    // si ocupas "vigencia", mapea aquí (por fechas/estado)
                };

                var data = await _api.ListarServiciosHuespedAsync(filtro, token)
                           ?? new List<ServicioHuespedDto>();

                // IMPORTANTE: la vista parcial espera IEnumerable<ServicioHuespedDto>
                return PartialView(
                    "~/Views/Huesped/Servicio/_TablaSolicitudesServicio.cshtml",
                    data
                );
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[TablaServicio] Error: {ex}");
                return new HttpStatusCodeResult(
                    (int)HttpStatusCode.InternalServerError,
                    "Error al cargar el listado de servicios."
                );
            }
        }

        // Eliminar una solicitud de servicio del huésped
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarServicio(int idServicio)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada");

                // idServicio = IdSolicitudServicio del DTO
                var ok = await _api.EliminarServicioHuespedAsync(idServicio, token);
                if (!ok)
                    return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "No se pudo eliminar.");

                return new HttpStatusCodeResult((int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[EliminarServicio] Error: {ex}");
                return new HttpStatusCodeResult(
                    (int)HttpStatusCode.InternalServerError,
                    "Error al eliminar el servicio."
                );
            }
        }

        // ================= WRAPPERS DE COMPATIBILIDAD =================

        // Dashboard superior (KPIs + gráfico) – por ahora vacío
        [HttpGet]
        public Task<ActionResult> DashboardServicio(DateTime? desde, DateTime? hasta)
        {
            var dto = new ServicioDashboardDto(); // DTO vacío, la vista puede mostrar 0 o placeholders

            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                {
                    Trace.TraceWarning("[DashboardServicio] Sin token. Se muestran datos por defecto.");
                }
                else
                {
                    // Implementar cuando tengas API real de dashboard
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DashboardServicio] Error: {ex}");
            }

            return Task.FromResult<ActionResult>(PartialView("_DashboardServicio", dto));
        }

        // Alias para vistas antiguas
        [HttpGet]
        public Task<ActionResult> Dashboard(DateTime? desde, DateTime? hasta)
            => DashboardServicio(desde, hasta);

        // Alias de tabla antigua
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> TablaPartial(int? vigencia, string nombre, bool? vip, int? capacidadMin)
            => TablaServicio(vigencia, nombre);

        // Alias de eliminar antiguo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> Eliminar(int idServicio)
            => EliminarServicio(idServicio);
    }
}
