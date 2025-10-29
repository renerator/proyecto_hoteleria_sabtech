using Front_Hoteleria.Dto.Reserva;
using Front_Hoteleria.Services.ReclamosHuesped;
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
    public class ReclamosHuespedController : Controller
    {
        private readonly IReclamosHuespedService _api;

        public ReclamosHuespedController() : this(new ReclamosHuespedService()) { }
        public ReclamosHuespedController(IReclamosHuespedService api) { _api = api; }

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
                case 1: return View("~/Views/Reservas/Index.cshtml");
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
            return PartialView("~/Views/Reservas/_TablaReserva.cshtml", data);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError($"[TablaReserva] {ex}");
            return new HttpStatusCodeResult(500, "Error al cargar reservas");
        }
    }
        [HttpGet]
        public async Task<ActionResult> Upsert()
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult(401);

            // Rellena combos (ajusta a tu servicio)
            ViewBag.Habitaciones = new SelectList(new[]
                {
        new { Id = 1, Nombre = "Individual" },
        new { Id = 2, Nombre = "Grupal" },
        new { Id = 3, Nombre = "Corporativa" }
    }, "Id", "Nombre");
            ViewBag.TiposReserva = new SelectList(new[]
            {
        new { Id = 1, Nombre = "Individual" },
        new { Id = 2, Nombre = "Grupal" },
        new { Id = 3, Nombre = "Corporativa" }
    }, "Id", "Nombre");

            ViewBag.IdTrabajador = Session["IdTrabajador"];

            var Dto = new Front_Hoteleria.Dto.Reserva.ReservaTrabajadorDto
            {
                FechaDesde = DateTime.Today,
                FechaHasta = DateTime.Today.AddDays(1),
                IdEstadoReserva = 1
            };

            return PartialView("~/Views/Reservas/_UpsertReserva.cshtml", Dto);
        }


        // ReservasController

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearReserva(Front_Hoteleria.Dto.Reserva.ReservaTrabajadorDto dto)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada");

                // saneos mínimos
                if (dto == null) return new HttpStatusCodeResult(400, "Datos inválidos");
                if (dto.IdHabitacion <= 0  || !dto.FechaDesde.HasValue || !dto.FechaHasta.HasValue)
                    return new HttpStatusCodeResult(400, "Campos obligatorios faltantes");

                if (dto.IdEstadoReserva == 0) dto.IdEstadoReserva = 1; // Ingresada

                var ok = await _api.CrearReservaTrabajadorAsync(dto, token);
                if (!ok) return new HttpStatusCodeResult(500, "No se pudo crear la reserva.");

                // Respuesta para manejar por JS
                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"[CrearReserva] {ex}");
                return new HttpStatusCodeResult(500, "Error al crear la reserva.");
            }
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
                return PartialView("~/Views/Reservas/_DashboardReserva.cshtml", dto);
                
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
