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
using System.Web;
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
        
        [HttpGet]
        public async Task<ActionResult> ControlCalidad()
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                // TODO: aquí deberías llamar a tu API real cuando la tengas:
                // var pendientes = await _OrdenApi.GetPendientesControlCalidadAsync(token);

                // Por ahora armamos un modelo de prueba similar al pantallazo:
                var model = new ControlCalidadDto
                {
                    PorcentajeAprobadas = 85m,
                    PorcentajeRechazadas = 10m,
                    PorcentajeRetrabajo = 5m,
                    TiempoPromedioMinutos = 138  // ~2h 18m (lo verás como 2h 18m)
                };

                model.ReparacionesPendientes.Add(new ReparacionCalidadItemDto
                {
                    IdReparacion = 1,
                    CodigoReparacion = "REP-001",
                    Descripcion = "Fuga en grifo del baño",
                    Habitacion = "Habitación 101",
                    ReportadoPor = "Carlos Rodríguez",
                    TiempoMinutos = 150 // 2h 30m
                });

                model.ReparacionesPendientes.Add(new ReparacionCalidadItemDto
                {
                    IdReparacion = 2,
                    CodigoReparacion = "REP-002",
                    Descripcion = "Lámpara de techo no funciona",
                    Habitacion = "Habitación 102",
                    ReportadoPor = "Luis Fernández",
                    TiempoMinutos = 75 // 1h 15m
                });

                model.ReparacionesPendientes.Add(new ReparacionCalidadItemDto
                {
                    IdReparacion = 5,
                    CodigoReparacion = "REP-005",
                    Descripcion = "Filtración de agua en techo",
                    Habitacion = "Habitación 202",
                    ReportadoPor = "María González",
                    TiempoMinutos = 260 // 4h 20m
                });

                return PartialView("~/Views/Habitaciones/_ControlCalidad.cshtml", model);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ControlCalidad-GET] Error inesperado: {ex}");
                return new HttpStatusCodeResult(
                    (int)HttpStatusCode.InternalServerError,
                    "Error al cargar el panel de control de calidad."
                );
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


        // GET: /OrdenTrabajo/Crear
        // GET: /OrdenTrabajo/NuevaOrdenTrabajo
        [HttpGet]
        public ActionResult NuevaOrdenTrabajo(int? idHabitacion)
        {
            var model = new OrdenTrabajoDto
            {
                // Aquí llenas los combos si es necesario
                // Habitaciones = _servicio.HabitacionesSelectList(),
                // TiposTrabajo = _servicio.TiposTrabajoSelectList(),
                // Prioridades  = _servicio.PrioridadesSelectList(),
                // Tecnicos     = _servicio.TecnicosSelectList(),
                // Contactos    = _servicio.ContactosSelectList()
            };

            // Asegúrate que el partial está en Views/OrdenTrabajo/_NuevaOrdenTrabajo.cshtml
            return PartialView("_NuevaOrdenTrabajo", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearOrden(OrdenTrabajoDto dto)
        {
            if (!ModelState.IsValid)
                return PartialView("_NuevaOrdenTrabajo", dto);

            // TODO: aquí llamas a tu API backend para crear la orden
            // var ok = _service.CrearOrden(dto);

            var ok = true;

            // Como se llama desde AJAX, devolvemos JSON
            return Json(new
            {
                ok,
                message = ok ? "Orden creada correctamente." : "No se pudo crear la orden."
            });
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
                //var dto = await _habInsumoApi.ListarAsync(idInventario, token);
                var dto = new InventarioHabitacionDTO {IdInventario=idInventario };

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

        // ===================== AUDITORÍA MATERIAL (GET PARCIAL) =====================
        [HttpGet]
        public ActionResult AuditoriaMaterial(int idInventario)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                // TODO: traer datos reales de inventario si lo necesitas
                var modelo = new AuditoriaInventarioDto
                {
                    IdInventario = idInventario,
                    FechaAuditoria = DateTime.Today,
                    HoraAuditoria = DateTime.Now.TimeOfDay,
                    TieneFotografias = false,
                    RequiereAccionCorrectiva = false
                };

                // Combos (por ahora vacíos para que no reviente)
                ViewBag.Estados = new List<SelectListItem>();
                ViewBag.Auditores = new List<SelectListItem>();

                return PartialView("~/Views/Habitaciones/_AuditoriaMaterial.cshtml", modelo);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[AuditoriaMaterial-GET] Error inesperado: {ex}");
                return new HttpStatusCodeResult(
                    (int)HttpStatusCode.InternalServerError,
                    "Error al cargar el formulario de auditoría."
                );
            }
        }
        /// <summary>
        /// Muestra el modal de carga masiva de habitaciones.
        /// GET: /Habitaciones/CargaMasivaHabitaciones
        /// </summary>
        // ===================== CARGA MASIVA HABITACIONES (VIEW) =====================
        [HttpGet]
        public ActionResult CargaMasivaHabitacionesView()
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult(
                        (int)HttpStatusCode.Unauthorized,
                        "Sesión expirada o sin autenticación.");

                // Nombre del template (en /Content/templates/)
                var nombreTemplate = "Template_CargaHabitaciones.xlsx";

                var modelo = new CargaMasivaHabitacionesDto
                {
                    NombreArchivoTemplate = nombreTemplate,
                    ProcesadoOk = false,
                    Mensaje = null
                };

                return PartialView(
                    "~/Views/Habitaciones/_CargaMasivaHabitaciones.cshtml",
                    modelo
                );
            }
          
            catch (Exception ex)
            {
                Trace.TraceError($"[CargaMasivaHabitacionesView-GET] Error inesperado: {ex}");
                return new HttpStatusCodeResult(
                    (int)HttpStatusCode.InternalServerError,
                    "Error al cargar el formulario de carga masiva de habitaciones.");
            }
        }


        /// <summary>
        /// Procesa el archivo de carga masiva.
        /// POST: /Habitaciones/CargaMasivaHabitaciones
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CargaMasivaHabitaciones(HttpPostedFileBase archivo)
        {
            if (archivo == null || archivo.ContentLength == 0)
                return Json(new { ok = false, message = "Debe seleccionar un archivo." });

            var resultado = new CargaMasivaHabitacionesResultadoDto();

            try
            {
                // TODO: leer el Excel, por cada fila construir un CargaMasivaHabitacionFilaDto,
                // validar y si es correcta, mapear a HabitacionDto y llamar a tu API.

                // Ejemplo ficticio de una sola fila:
                var filaEjemplo = new CargaMasivaHabitacionFilaDto
                {
                    NumeroFila = 2,
                    CodigoHabitacion = "H101",
                    NombreHabitacion = "Habitación 101 Norte",
                    TipoHabitacion = "Single",
                    Capacidad = 1,
                    EsVip = false,
                    Precio = 0,
                    EstadoTexto = "Activa",
                    Observaciones = "Sin observaciones",
                    EsValida = true
                };

                resultado.Detalle.Add(filaEjemplo);
                resultado.TotalFilas = 1;
                resultado.FilasCorrectas = 1;
                resultado.FilasConError = 0;

                return Json(new
                {
                    ok = true,
                    message = $"Carga masiva realizada. Filas OK: {resultado.FilasCorrectas}, con error: {resultado.FilasConError}"
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[CargaMasivaHabitaciones] Error: {ex}");
                return Json(new { ok = false, message = "Error al procesar el archivo de carga masiva." });
            }
        }


        // ===================== EDITAR INVENTARIO (GET PARCIAL) =====================
        [HttpGet]
        public async Task<ActionResult> EditarInventario(int idInventario)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                // TODO: traer el registro real desde tu API.
                // Ejemplo futuro:
                // var dto = await _habInsumoApi.ObtenerPorIdAsync(idInventario, token)
                //               ?? new InventarioHabitacionDTO();

                var dto = new InventarioHabitacionDTO
                {
                    IdInventario = idInventario
                };

                // Combos de ejemplo (de momento vacíos)
                ViewBag.Habitaciones = new List<SelectListItem>();
                ViewBag.TiposMaterial = new List<SelectListItem>();
                ViewBag.Estados = new List<SelectListItem>();

                return PartialView("~/Views/Habitaciones/_EditarInventario.cshtml", dto);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[EditarInventario-GET] Error inesperado: {ex}");
                return new HttpStatusCodeResult(
                    (int)HttpStatusCode.InternalServerError,
                    "Error al cargar el formulario de inventario."
                );
            }
        }

        // ===================== ELIMINAR INVENTARIO HABITACIÓN =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarInventario(int idHabitacionInsumo)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                // TODO: llamar a tu API de backend para eliminar realmente el registro.
                // Ejemplo (ajusta cuando tengas el método):
                // var ok = await _habInsumoApi.EliminarAsync(idHabitacionInsumo, token);
                var ok = true;

                if (!ok)
                    return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "No se pudo eliminar el material.");

                return new HttpStatusCodeResult((int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[EliminarInventario] Error inesperado: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al eliminar el material de la habitación.");
            }
        }

        // ===================== DETALLE REPARACIÓN (MODAL) =====================
        [HttpGet]
        public ActionResult DetalleReparacion(int idReparacion)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                // TODO: aquí deberías llamar a tu API para traer el detalle real
                // Ejemplo futuro:
                // var dto = await _OrdenApi.ObtenerDetalleAsync(idReparacion, token);
                // if (dto == null) return HttpNotFound("No se encontró la reparación.");

                var dto = new ReparacionDetalleDto
                {
                    IdReparacion = idReparacion
                };

                return PartialView("~/Views/Habitaciones/_DetalleReparacion.cshtml", dto);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DetalleReparacion-GET] Error inesperado: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al cargar el detalle de la reparación.");
            }
        }
        // ===================== EDITAR REPARACIÓN (MODAL) =====================
        [HttpGet]
        public ActionResult EditarReparacion(int idReparacion)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                // TODO: obtener datos reales desde la API para edición.
                // var dto = await _OrdenApi.ObtenerParaEdicionAsync(idReparacion, token) ?? new ReparacionEditDto();
                var dto = new ReparacionEditDto
                {
                    IdReparacion = idReparacion
                };

                // Si tu partial usa combos, aquí puedes setear ViewBag.* con listas vacías por ahora
                ViewBag.Habitaciones = new List<SelectListItem>();
                ViewBag.Estados = new List<SelectListItem>();
                ViewBag.Tipos = new List<SelectListItem>();
                ViewBag.Tecnicos = new List<SelectListItem>();
                ViewBag.Prioridades = new List<SelectListItem>();

                return PartialView("~/Views/Habitaciones/_EditarReparacion.cshtml", dto);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[EditarReparacion-GET] Error inesperado: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al cargar el formulario de reparación.");
            }
        }
        // ===================== ASIGNAR TÉCNICO (MODAL) =====================
        [HttpGet]
        public ActionResult AsignarTecnico(int idReparacion)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                // TODO: cuando tengas API, trae los datos reales de la reparación/asignación.
                // var dto = await _OrdenApi.ObtenerAsignacionTecnicoAsync(idReparacion, token) ?? new AsignacionTecnicoDto();
                var dto = new AsignacionTecnicoDto
                {
                    IdReparacion = idReparacion,
                    CodigoReparacion = idReparacion > 0 ? $"REP-{idReparacion:000000}" : string.Empty,
                    FechaAsignacion = DateTime.Today,
                    HoraInicio = DateTime.Now.ToString("HH:mm"),
                    TiempoEstimadoHoras = 1
                };

                // Combo de técnicos (por ahora vacío para que no reviente)
                ViewBag.Tecnicos = new List<SelectListItem>();
                // Ejemplo futuro:
                // ViewBag.Tecnicos = await _OrdenApi.ObtenerTecnicosSelectAsync(token);

                return PartialView("~/Views/Habitaciones/_AsignarTecnicoReparacion.cshtml", dto);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[AsignarTecnico-GET] Error inesperado: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al cargar el formulario de asignación de técnico.");
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
