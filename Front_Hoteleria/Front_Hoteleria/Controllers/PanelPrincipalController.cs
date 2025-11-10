using Front_Hoteleria.Dto.Reserva;
using Front_Hoteleria.Services.PanelPrincipal;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class PanelPrincipalController : Controller
    {
        private readonly IPanelPrincipalService _api;

        public PanelPrincipalController() : this(new PanelPrincipalService()) { }
        public PanelPrincipalController(IPanelPrincipalService api) { _api = api; }

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
        // GET: /PanelPrincipal/Detalle?id=RES-001
        [HttpGet]
        public ActionResult Detalle(string id)
        {
            // Aquí deberías ir a tu servicio/repositorio para traer la reserva.
            // Como no me pasaste el DTO real, te armo uno “mock” para que el modal no reviente.
            // Luego lo cambias por tu llamada real.

            var modelo = new ReservaTrabajadorDto();
            
            return PartialView("~/Views/PanelPrincipal/_DetalleReserva.cshtml", modelo);
        }

        [HttpGet]
        public async Task<ActionResult> Editar(string id)
        {
            // aquí deberías ir a la API real
            // por ahora te hago el mismo mock que en Detalle
            // para que veas el modal con datos

            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult(401, "Sesión expirada");

            // si tu servicio tuviera algo como ObtenerReservaPorIdAsync(id, token)
            // var dto = await _api.ObtenerReservaPorIdAsync(id, token);

            var dto = new ReservaTrabajadorDto();
            

            return PartialView("~/Views/PanelPrincipal/_EditarReserva.cshtml", dto);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var tok = Session["Token"] as string;
            if (string.IsNullOrWhiteSpace(tok))
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            if (Session["IdPerfil"] == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            return View("~/Views/PanelPrincipal/Index.cshtml");
        }

        // ===== PARCIAL DASHBOARD (respeta las 00:00 y 23:59)
        [HttpGet]
        public async Task<ActionResult> Dashboard(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                //var baseDesde = (fechaDesde ?? DateTime.Today).Date;
                //var baseHasta = (fechaHasta ?? DateTime.Today).Date;
                //var desde = new DateTime(baseDesde.Year, baseDesde.Month, baseDesde.Day, 0, 0, 0);
                //var hasta = new DateTime(baseHasta.Year, baseHasta.Month, baseHasta.Day, 23, 59, 0);
                //desde = null;
                //hasta = null;
                var dto = await _api.DashboardReservasPanelPrincipalAsync(fechaDesde, fechaHasta, token)
                          ?? new ReservaDashboardPanelPrincipalDto();

                return PartialView("~/Views/PanelPrincipal/_DashboardAdm.cshtml", dto);
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[Dashboard] HTTP: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.BadGateway, "No se pudo comunicar con la API de dashboard.");
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[Dashboard] Timeout: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.GatewayTimeout, "La consulta de dashboard excedió el tiempo de espera.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Dashboard] Error: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al cargar el dashboard.");
            }
        }

        // ===== PARCIAL TABLA (nombre esperado por tu Index.js: "Tabla")
        [HttpGet]
        public async Task<ActionResult> Tabla(DateTime? fechaDesde, DateTime? fechaHasta, int? idEstadoReserva, int? idTipoReserva)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult(401, "Sesión expirada");

                var filtro = new ReservaTrabajadorDto
                {
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta,
                    IdEstadoReserva = idEstadoReserva ?? 0,
                    IdTipoReserva = idTipoReserva ?? 0
                };

                var data = await _api.ReservasDisponiblesTrabajadorAsync(filtro, token)
                           ?? new System.Collections.Generic.List<ReservaTrabajadorDto>();

                // Vista correcta en /Views/PanelPrincipal
                return PartialView("~/Views/PanelPrincipal/_TablaDashAdm.cshtml", data);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Tabla] {ex}");
                return new HttpStatusCodeResult(500, "Error al cargar reservas");
            }
        }

        // ===== PARCIAL MODAL
        [HttpGet]
        public PartialViewResult Upsert()
        {
            return PartialView("~/Views/PanelPrincipal/_UpsertDashAdm.cshtml");
        }
    }
}
