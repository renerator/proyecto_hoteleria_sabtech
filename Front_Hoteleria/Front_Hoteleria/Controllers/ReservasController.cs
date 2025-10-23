using Front_Hoteleria.Dto.adm.Reserva;
using Front_Hoteleria.Services.Reservas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Front_Hoteleria.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IReservaService _api;

        public ReservasController() : this(new ReservaService()) { }
        public ReservasController(IReservaService api) { _api = api; }

        // -------------------------------
        //  TOKEN & PERFIL
        // -------------------------------
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

        // -------------------------------
        //  INDEX
        // -------------------------------
        [HttpGet]
        public ActionResult Index()
        {
            if (!(Session["Token"] is string tok) || string.IsNullOrWhiteSpace(tok))
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            var perfil = Session["IdPerfil"];
            if (perfil == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            switch (perfil)
            {
                case 1: return View("~/Views/adm/Reservas/Index.cshtml");
                case 2: return View("~/Views/Huesped/Reservas/Index.cshtml");
                default: return RedirectToAction("Login", "Account");
            }
        }



public async Task<ActionResult> TablaPartial(
    DateTime? fechaDesde,
    DateTime? fechaHasta,
    int? idEstadoReserva,
    int? idtiporeserva)
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
                IdEstadoReserva = idEstadoReserva ?? 0, // 0 = no filtra (según tu SP)
                IdTipoReserva = idtiporeserva ?? 0
            };

            var data = await _api.ReservasDisponiblesTrabajadorAsync(filtro, token);

            // TIP: el partial debe estar tipado a List<ReservaTrabajadorDto>
            return PartialView("~/Views/adm/Reservas/_TablaReserva.cshtml", data);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError($"[TablaReserva] {ex}");
            return new HttpStatusCodeResult(500, "Error al cargar reservas");
        }
    }


        // -------------------------------
        //  UPSERT (Modal)
        // -------------------------------
        [HttpGet]
        public async Task<ActionResult> Upsert(int? id)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized);

            var dto = new ReservaDto();
            if (id.HasValue)
            {
                var lista = await _api.ReservasDisponiblesAsync(1, token);
                dto = lista.FirstOrDefault(x => x.IdHabitacion == id.Value) ?? new ReservaDto();
            }

            return PartialView("_UpsertReserva", dto);
        }
        // ===== DASHBOARD VIEW (PARCIAL) =====
        // SIN parámetros de fecha
        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                // Llama al servicio SIN parámetros de fecha (usa null, null)

                var dto = new ReservaDashboardDto();
                 dto = await _api.DashboardReservasAsync(token)
                          ?? new ReservaDashboardDto();

                // Devuelve el parcial fuertemente tipado con el DTO
                return PartialView("~/Views/adm/Reservas/_DashboardReserva.cshtml", dto);
                
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[Dashboard] Error HTTP: {ex}");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Eliminar(int idHabitacion)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized);

                var ok = await _api.EliminarReservaAsync(idHabitacion, token);
                if (!ok) return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "No se pudo eliminar.");
                return new HttpStatusCodeResult((int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Eliminar] Error: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al eliminar la reserva.");
            }
        }
    }
}
