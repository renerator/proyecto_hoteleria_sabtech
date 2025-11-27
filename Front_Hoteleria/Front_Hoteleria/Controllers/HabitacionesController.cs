using Front_Hoteleria.Dto.Habitacion;
using Front_Hoteleria.Dto.Inventario;
using Front_Hoteleria.Dto.OrdenTrabajo;

using Front_Hoteleria.Dtos.Habitacion;
using Front_Hoteleria.Services.Habitacion;
using Front_Hoteleria.Services.HabitacionInventario;
using Front_Hoteleria.Services.OrdenTrabajo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class HabitacionesController : Controller
    {
        private readonly IHabitacionService _api;
        private readonly IHabitacionInventarioService _habInsumoApi;
        private readonly IOrdenTrabajoService _OrdenApi;

        public HabitacionesController(
            IHabitacionService api,
            IHabitacionInventarioService habInsumoApi,
            IOrdenTrabajoService OrdenApi)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _habInsumoApi = habInsumoApi ?? throw new ArgumentNullException(nameof(habInsumoApi));
            _OrdenApi = OrdenApi ?? throw new ArgumentNullException(nameof(OrdenApi));
        }

        public HabitacionesController()
            : this(new HabitacionService(), new HabitacionInventarioService(), new OrdenTrabajoService())
        { }

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

        // ===================== INDEX + KPIs REPARACIONES =====================
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            if (!(Session["Token"] is string tok) || string.IsNullOrWhiteSpace(tok))
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            try
            {
                var perfil = Session["IdPerfil"];
                if (perfil == null)
                    return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

                // Inicializamos KPIs en cero por si algo falla
                ViewBag.RepPendientes = 0;
                ViewBag.RepEnProgreso = 0;
                ViewBag.RepCompletadas = 0;
                ViewBag.RepUrgentes = 0;
                ViewBag.RepHoy = 0;
                ViewBag.RepSlaVencido = 0;
                ViewBag.RepTotal = 0;

                // Solo para el perfil que ve esta vista
                switch (perfil)
                {
                    case 4:
                        await CargarKpisReparacionesAsync();
                        return View("~/Views/Habitaciones/Index.cshtml");

                    default:
                        return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error en Index: {ex}");
                return RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// Calcula las estadísticas de reparaciones (pendientes, en progreso, etc.)
        /// usando LINQ sobre el listado que entrega el servicio de órdenes.
        /// </summary>
        private async Task CargarKpisReparacionesAsync()
        {
            try
            {
                var bearer = GetBearer();
                if (string.IsNullOrWhiteSpace(bearer))
                    return;

                // IMPORTANTE:
                // Si GetListaOrdenTrabajoEstadoAsync filtra por estado,
                // usa el valor que te traiga TODAS las órdenes vigentes.
                // Aquí dejo 1 como en TablaReparaciones; ajusta si tu API espera otro valor.
                var ordenes = await _OrdenApi.GetListaOrdenTrabajoEstadoAsync(1, bearer)
                                             .ConfigureAwait(false);

                if (ordenes == null)
                    ordenes = new List<OrdenTrabajoDto>();

                Func<string, string> norm = s =>
                    (s ?? string.Empty)
                        .ToLowerInvariant()
                        .Normalize(NormalizationForm.FormD)
                        .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                        .Aggregate(string.Empty, (acc, c) => acc + c);

                var hoy = DateTime.Today;

                // ---- ESTADO ----
                int pendientes = ordenes.Count(o =>
                {
                    var e = norm(o.Estado);
                    return e.Contains("pend");
                });

                int enProgreso = ordenes.Count(o =>
                {
                    var e = norm(o.Estado);
                    return e.Contains("progres") || e.Contains("curso");
                });

                int completadas = ordenes.Count(o =>
                {
                    var e = norm(o.Estado);
                    return e.Contains("complet") || e.Contains("cerrad") || e.Contains("terminad");
                });

                // ---- PRIORIDAD / URGENTES ----
                int urgentes = ordenes.Count(o =>
                {
                    var p = norm(o.Prioridad);
                    return p.Contains("urgente") || p.Contains("alto");
                });

                // ---- HOY (por fecha de creación / solicitud) ----
                int hoyCnt = ordenes.Count(o => o.FechaIngresoOT.Date == hoy);

                // ---- SLA VENCIDO ----
                // Regla: solo cuenta órdenes NO completadas con fecha SLA < hoy
                int slaVencido = ordenes.Count(o =>
                {
                    var e = norm(o.Estado);
                    bool esCompleta = e.Contains("complet") || e.Contains("cerrad") || e.Contains("terminad");

                    if (esCompleta) return false;
                    if (!o.FechaCierreOT.HasValue) return false;   // 👈 aquí sí uso HasValue

                    return o.FechaCierreOT.Value.Date < hoy;
                });

                ViewBag.RepPendientes = pendientes;
                ViewBag.RepEnProgreso = enProgreso;
                ViewBag.RepCompletadas = completadas;
                ViewBag.RepUrgentes = urgentes;
                ViewBag.RepHoy = hoyCnt;
                ViewBag.RepSlaVencido = slaVencido;
                ViewBag.RepTotal = ordenes.Count;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[CargarKpisReparacionesAsync] Error calculando KPIs: {ex}");
                // Si falla, dejamos los ViewBag en cero (ya inicializados en Index)
            }
        }

        // ===================== LISTADO HABITACIONES =====================
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
                    data = data.Where(x => (x.NombreHabitacion ?? string.Empty)
                                    .IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                if (vip.HasValue)
                    data = data.Where(x => x.VIP == vip.Value).ToList();

                if (capacidadMin.HasValue)
                    data = data.Where(x => x.Capacidad >= capacidadMin.Value).ToList();

                return PartialView("~/Views/Habitaciones/_TablaHabitaciones.cshtml", data);
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

        // ===================== DASHBOARD =====================
        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                var dto = await _api.DashboardHabitacionAsync(token) ?? new HabitacionDashboardDto();
                return PartialView("~/Views/Habitaciones/_DashboardHabitacion.cshtml", dto);
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

        // ===================== FILTROS INVENTARIO =====================
        [HttpGet]
        public async Task<JsonResult> FiltrosInventario()
        {
            try
            {
                var bearer = GetBearer();
                var data = await _habInsumoApi.ListarAsync(1, bearer).ConfigureAwait(false);

                var habs = data
                    .Select(x => x.NombreHabitacion)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(x => new { value = x, text = x })
                    .ToList();

                var ins = data
                    .Select(x => x.Descripcion)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(x => new { value = x, text = x })
                    .ToList();

                return Json(new { habitaciones = habs, insumos = ins }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[FiltrosInventario] Error: {ex}");
                return Json(new { habitaciones = new object[0], insumos = new object[0] }, JsonRequestBehavior.AllowGet);
            }
        }

        // ===================== TABLA INVENTARIO =====================
        [HttpGet]
        public async Task<ActionResult> TablaInventario(string habitacion, string material)
        {
            try
            {
                var bearer = GetBearer();
                var data = await _habInsumoApi.ListarAsync(1, bearer).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(habitacion))
                    data = data
                        .Where(d => string.Equals(d.NombreHabitacion, habitacion, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                if (!string.IsNullOrWhiteSpace(material))
                    data = data
                        .Where(d => (d.Descripcion ?? string.Empty)
                            .IndexOf(material, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();

                return PartialView("_TablaInventario", data);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[TablaInventario] Error: {ex}");
                return PartialView("_TablaInventario", new List<InventarioHabitacionDTO>());
            }
        }

        // ===================== TABLA REPARACIONES =====================
        [HttpGet]
        public async Task<ActionResult> TablaReparaciones()
        {
            try
            {
                var bearer = GetBearer();
                if (string.IsNullOrWhiteSpace(bearer))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                var data = await _OrdenApi.GetListaOrdenTrabajoEstadoAsync(1, bearer).ConfigureAwait(false);

                return PartialView("_TablaReparaciones", data);
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[TablaReparaciones] Error HTTP al consultar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.BadGateway, "No se pudo comunicar con la API de órdenes de trabajo.");
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[TablaReparaciones] Timeout al consultar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.GatewayTimeout, "La consulta de órdenes excedió el tiempo de espera.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[TablaReparaciones] Error inesperado: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al cargar las órdenes de trabajo.");
            }
        }

        // ===================== UPSERT HABITACIÓN =====================
        [HttpGet]
        public async Task<ActionResult> Upsert(int? id, bool? soloLectura)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                var dto = new HabitacionDto { Capacidad = 1, IdEstado = 1 };

                if (id.HasValue)
                {
                    var lista = await _api.HabitacionesDisponiblesAsync(1, token);
                    var existente = lista.FirstOrDefault(x => x.IdHabitacion == id.Value);
                    if (existente != null) dto = existente;
                }

                ViewBag.SoloLectura = soloLectura ?? false;

                return PartialView("~/Views/Habitaciones/_UpsertHabitacion.cshtml", dto);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Upsert-GET] Error inesperado: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al cargar el formulario.");
            }
        }

        public ActionResult CrearHabitacion()
        {
            var modelo = new HabitacionDto
            {
                IdEstado = 1,
                Capacidad = 1,
                Precio = 0
            };
            return PartialView("~/Views/Habitaciones/_CrearHabitacion.cshtml", modelo);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upsert(HabitacionDto dto)
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
        [HttpGet]
        public async Task<ActionResult> DetalleInventario(int idInventario)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                // Llamas a tu API para obtener el detalle
                var dto = await _habInsumoApi.ListarAsync(idInventario, token);
                if (dto == null)
                    return HttpNotFound("No se encontró el material solicitado.");

                // Partial que dibuja el modal (solo el cuerpo)
                return PartialView("_DetalleInventario", dto);
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[DetalleInventario] Error HTTP al llamar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.BadGateway, "No se pudo comunicar con la API.");
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[DetalleInventario] Timeout al llamar API: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.GatewayTimeout, "La operación excedió el tiempo de espera.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DetalleInventario] Error inesperado: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al obtener el detalle del material.");
            }
        }

        // ===================== ELIMINAR HABITACIÓN =====================
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
