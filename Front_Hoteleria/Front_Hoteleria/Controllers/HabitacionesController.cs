using Front_Hoteleria.Dto.adm.Habitacion;
using Front_Hoteleria.Services.adm.Habitacion;
using Front_Hoteleria.Services.HabitacionInsumo;
using Front_Hoteleria.Models.Habitacion;
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
    public class HabitacionesController : Controller
    {
        private readonly IHabitacionService _api;
        private readonly IHabitacionInsumoService _habInsumoApi;

        // ----- DI (recomendado) -----
        public HabitacionesController(IHabitacionService api, IHabitacionInsumoService habInsumoApi)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _habInsumoApi = habInsumoApi ?? throw new ArgumentNullException(nameof(habInsumoApi));
        }

        // ----- Fallback sin contenedor de DI (opcional) -----
        public HabitacionesController() : this(new HabitacionService(), new HabitacionInsumoService()) { }

        // ----- Helper para obtener el Bearer -----
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
        public ActionResult Index()
        {
            // Verifica si existe token válido en sesión
            if (!(Session["Token"] is string tok) || string.IsNullOrWhiteSpace(tok))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
            }

            try
            {
                var perfil = Session["IdPerfil"];
                if (perfil == null)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
                }

                // Suponiendo que Usuario tiene una propiedad IdPerfil o Rol
                // Ejemplo: Rol = "Administrador", "Huesped", "Personal"
                switch (perfil)
                {
                    case 1:
                        // Redirige a la vista de administrador
                        return View("~/Views/adm/Habitaciones/Index.cshtml");

                    case 2:
                        // Redirige a la vista específica del huésped
                        return View("~/Views/Huesped/Reservas/Index.cshtml");



                    default:
                        // Cualquier otro caso no autorizado
                        return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                // Log opcional
                System.Diagnostics.Trace.TraceError($"Error en Index: {ex}");
                return RedirectToAction("Login", "Account");
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

                return PartialView("~/Views/adm/Habitaciones/_TablaHabitaciones.cshtml", data);
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

        // ===================== DASHBOARD (SIN FECHAS) =====================
        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                var dto = await _api.DashboardHabitacionAsync(token) ?? new HabitacionDashboardDto();

                // Usamos ruta absoluta para evitar problemas de resolución del partial
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

        [HttpGet]
        public async Task<JsonResult> FiltrosInventario()
        {
            try
            {
                var bearer = GetBearer();
                var data = await _habInsumoApi.ListarAsync(1, bearer).ConfigureAwait(false);

                var habs = data.Select(x => x.IdHabitacion).Distinct()
                               .OrderBy(x => x)
                               .Select(x => new { value = x, text = x.ToString() })
                               .ToList();

                var ins = data.Select(x => new { x.IdInsumo, x.NombreInsumo })
                              .Distinct()
                              .OrderBy(x => x.NombreInsumo)
                              .Select(x => new { value = x.IdInsumo, text = x.NombreInsumo })
                              .ToList();

                return Json(new { habitaciones = habs, insumos = ins }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[FiltrosInventario] Error: {ex}");
                return Json(new { habitaciones = new object[0], insumos = new object[0] }, JsonRequestBehavior.AllowGet);
            }
        }

        // Tabla inventario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> TablaInventario(FiltroInventarioVm f)
        {
            try
            {
                var bearer = GetBearer();
                var data = await _habInsumoApi.ListarAsync(1, bearer).ConfigureAwait(false);

                if (f?.IdHabitacion != null) data = data.Where(d => d.IdHabitacion == f.IdHabitacion.Value).ToList();
                if (f?.IdInsumo != null) data = data.Where(d => d.IdInsumo == f.IdInsumo.Value).ToList();

                return PartialView("_TablaInventario", data);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[TablaInventario] Error: {ex}");
                return PartialView("_TablaInventario", new List<InventarioFilaVm>());
            }
        }

        // ===================== UPSERT HABITACIÓN =====================
        [HttpGet]
        public async Task<ActionResult> Upsert(int? id)
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
