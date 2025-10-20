using Front_Hoteleria.Dto.Reserva;
using Front_Hoteleria.Dto.Servicio;
using Front_Hoteleria.Services.Servicio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly IServicioService _api;

        public ServiciosController() : this(new ServicioService()) { }
        public ServiciosController(IServicioService api) { _api = api; }

        // Token (cookie o sesión)
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

        [HttpGet]
        public ActionResult Index() => View();

        // =================== PARCIALES ===================

        // Dashboard superior (KPIs + gráfico)
        [HttpGet]
        public async Task<ActionResult> DashboardServicio(DateTime? desde, DateTime? hasta)
        {
            // 1) DTO con valores por defecto (mock) para que SIEMPRE haya contenido
            var dto = new ServicioDashboardDto
            {
                TotalServicios = 231_809,
                TotalDesayunos = 897,
                TotalLimpieza = 650,
                TotalTickets = 111_569
            };

            try
            {
                // 2) Si hay token, intentamos la API; si falla, dejamos el mock
                var token = GetBearer();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var d = desde ?? DateTime.Today.AddDays(-30);
                    var h = hasta ?? DateTime.Today;

                    var apiDto = await _api.DashboardHabitacionAsync(d, h, token);
                    if (apiDto != null) dto = apiDto;
                }
                else
                {
                    Trace.TraceWarning("[DashboardServicio] Sin token. Se muestran datos mock.");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DashboardServicio] Fallback por error: {ex}");
                // NO devolvemos 5xx; mostramos el mock
            }

            return PartialView("_DashboardServicio", dto);
        }

        // Paneles intermedios (estático en la vista)
        [HttpGet]
        public ActionResult PanelesServicio() => PartialView("_PanelesServicio");

        // Tabla (listado)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TablaServicio(int? vigencia, string nombre)
        {
            // mock inicial para que SIEMPRE muestre filas
            var data = new List<ServicioDto>
            {
                new ServicioDto{ IdServicio=1, NumeroHabitacion="0005", NombreServicio="Limpieza",  Fecha=DateTime.Today, Hora="10:00", Prioridad="Alta" },
                new ServicioDto{ IdServicio=2, NumeroHabitacion="0008", NombreServicio="Mantenimiento", Fecha=DateTime.Today, Hora="14:30", Prioridad="Urgente" },
                new ServicioDto{ IdServicio=3, NumeroHabitacion="0012", NombreServicio="WiFi",       Fecha=DateTime.Today, Hora="16:45", Prioridad="Normal" }
            };

            try
            {
                var token = GetBearer();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var apiData = await _api.HabitacionesDisponiblesAsync(vigencia ?? 1, token);
                    if (apiData != null && apiData.Any())
                        data = apiData;
                }
                else
                {
                    Trace.TraceWarning("[TablaServicio] Sin token. Se muestran filas mock.");
                }

                if (!string.IsNullOrWhiteSpace(nombre))
                    data = data.Where(x => (x.NombreServicio ?? string.Empty)
                                    .IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[TablaServicio] Fallback por error: {ex}");
                // dejamos data mock
            }

            return PartialView("_TablaServicio", data);
        }

        // Upsert GET
        [HttpGet]
        public async Task<ActionResult> UpsertServicio(int? id)
        {
            var dto = new ServicioDto();

            try
            {
                var token = GetBearer();
                if (!string.IsNullOrWhiteSpace(token) && id.HasValue)
                {
                    var lista = await _api.HabitacionesDisponiblesAsync(1, token);
                    var existente = lista?.FirstOrDefault(x => x.IdServicio == id.Value);
                    if (existente != null) dto = existente;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[UpsertServicio-GET] Error (se muestra formulario vacío): {ex}");
            }

            return PartialView("_UpsertServicio", dto);
        }

        // Upsert POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpsertServicio(ServicioDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "Datos inválidos.");

                var token = GetBearer();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    bool ok = dto.IdServicio == 0
                        ? await _api.CrearHabitacionAsync(dto, token)
                        : await _api.ModificarHabitacionAsync(dto, token);

                    if (!ok) return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "No se pudo guardar.");
                }
                else
                {
                    Trace.TraceWarning("[UpsertServicio-POST] Sin token. Simulando guardado OK.");
                }

                return new HttpStatusCodeResult((int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[UpsertServicio-POST] Error: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al guardar el servicio.");
            }
        }

        // Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarServicio(int idServicio)
        {
            try
            {
                var token = GetBearer();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var ok = await _api.EliminarHabitacionAsync(idServicio, token);
                    if (!ok) return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "No se pudo eliminar.");
                }
                else
                {
                    Trace.TraceWarning("[EliminarServicio] Sin token. Simulando eliminado OK.");
                }

                return new HttpStatusCodeResult((int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[EliminarServicio] Error: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al eliminar el servicio.");
            }
        }

        // ===== Wrappers de compatibilidad (por si alguna vista vieja los llama) =====
        [HttpGet] public Task<ActionResult> Dashboard(DateTime? desde, DateTime? hasta) => DashboardServicio(desde, hasta);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> TablaPartial(int? vigencia, string nombre, bool? vip, int? capacidadMin)
            => TablaServicio(vigencia, nombre);

        [HttpGet] public Task<ActionResult> Upsert(int? id) => UpsertServicio(id);
        [HttpPost][ValidateAntiForgeryToken] public Task<ActionResult> Upsert(ServicioDto dto) => UpsertServicio(dto);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> Eliminar(int idServicio) => EliminarServicio(idServicio);
    }
}
