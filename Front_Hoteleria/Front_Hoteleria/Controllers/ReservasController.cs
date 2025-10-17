using Front_Hoteleria.Dto.Reserva;
using Front_Hoteleria.Services.Reserva;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IReservaService _api;

        public ReservasController() : this(new ReservaService()) { }
        public ReservasController(IReservaService api) { _api = api; }

        // Helper unificado para leer el bearer
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
        public ActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TablaPartial(int? vigencia, string nombre, bool? vip, int? capacidadMin)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                var data = await _api.HabitacionesDisponiblesAsync(vigencia ?? 1, token);

                if (!string.IsNullOrWhiteSpace(nombre))
                    data = data.Where(x => (x.MotivoReserva ?? string.Empty)
                                .IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                //if (vip.HasValue)
                //    data = data.Where(x => x.VIP == vip.Value).ToList();

                //if (capacidadMin.HasValue)
                //    data = data.Where(x => x.Capacidad >= capacidadMin.Value).ToList();

                return PartialView("_TablaHabitaciones", data);
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[TablaPartial] Error HTTP al consultar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.BadGateway, "No se pudo comunicar con la API de habitaciones.");
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[TablaPartial] Timeout al consultar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.GatewayTimeout, "La consulta a la API excedió el tiempo de espera.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[TablaPartial] Error inesperado: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al cargar habitaciones.");
            }
        }

        // Dashboard (estilo Upsert: arma modelo y devuelve partial)
        [HttpGet]
        public async Task<ActionResult> Dashboard(DateTime? desde, DateTime? hasta)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                // Defaults de fecha (últimos 30 días) si vienen nulas
                var d = desde ?? DateTime.Today.AddDays(-30);
                var h = hasta ?? DateTime.Today;

                var dto = await _api.DashboardHabitacionAsync(d, h, token);
                dto = dto ?? new ReservaDashboardDto();

                return PartialView("_DashboardReserva", dto);
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[Dashboard] Error HTTP al consultar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.BadGateway, "No se pudo comunicar con la API de dashboard.");
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[Dashboard] Timeout al consultar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.GatewayTimeout, "La consulta de dashboard excedió el tiempo de espera.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Dashboard] Error inesperado: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al cargar el dashboard de habitaciones.");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Upsert(int? id)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                var dto = new ReservaDto();

                if (id.HasValue)
                {
                    var lista = await _api.HabitacionesDisponiblesAsync(1, token);
                    var existente = lista.FirstOrDefault(x => x.IdHabitacion == id.Value);
                    if (existente != null) dto = existente;
                }

                return PartialView("_UpsertHabitacion", dto);
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[Upsert-GET] Error HTTP al consultar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.BadGateway, "No se pudo comunicar con la API.");
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[Upsert-GET] Timeout al consultar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.GatewayTimeout, "La consulta excedió el tiempo de espera.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Upsert-GET] Error inesperado: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al cargar el formulario.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upsert(ReservaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "Datos inválidos.");

                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                bool ok = dto.IdHabitacion == 0
                    ? await _api.CrearHabitacionAsync(dto, token)
                    : await _api.ModificarHabitacionAsync(dto, token);

                if (!ok) return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "No se pudo guardar.");
                return new HttpStatusCodeResult((int)HttpStatusCode.OK);
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[Upsert-POST] Error HTTP al llamar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.BadGateway, "No se pudo comunicar con la API.");
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[Upsert-POST] Timeout al llamar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.GatewayTimeout, "La operación excedió el tiempo de espera.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Upsert-POST] Error inesperado: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al guardar la habitación.");
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
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                var ok = await _api.EliminarHabitacionAsync(idHabitacion, token);
                if (!ok) return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "No se pudo eliminar.");
                return new HttpStatusCodeResult((int)HttpStatusCode.OK);
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[Eliminar] Error HTTP al llamar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.BadGateway, "No se pudo comunicar con la API.");
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[Eliminar] Timeout al llamar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.GatewayTimeout, "La operación excedió el tiempo de espera.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Eliminar] Error inesperado: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al eliminar la habitación.");
            }
        }
    }
}
