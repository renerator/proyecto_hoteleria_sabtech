using Front_Hoteleria.Dto.Campamentos;
using Front_Hoteleria.Dto.Inventario;
using Front_Hoteleria.Services.Inventario;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class InventarioController : Controller
    {
        private readonly IInventarioService _api;

        public InventarioController() : this(new InventarioService()) { }

        public InventarioController(IInventarioService api)
        {
            _api = api;
        }

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
                Trace.TraceError("[InventarioController.GetBearer] " + ex);
                return null;
            }
        }

        // GET: /Inventario
        [HttpGet]
        public ActionResult Index()
        {
            // igual que contratos: solo devuelve la vista
            return View("~/Views/Inventario/Index.cshtml");
        }

        // PANEL (kpi)
        // GET: /Inventario/Resumen
        [HttpGet]
        public async Task<ActionResult> Resumen()
        {
            var token = GetBearer();

            var dto = await _api.ResumenAsync(token);
            if (dto == null)
            {
                dto = new InventarioKpiDto
                {
                    TotalItems = 156,
                    Disponibles = 142,
                    Faltantes = 3,
                    EnMantenimiento = 11
                };
            }

            // tu parcial: ~/Views/Inventario/_DashboardInventario.cshtml
            return PartialView("~/Views/Inventario/_DashboardInventario.cshtml", dto);
        }
        // GET: /Inventario/Ver?id=INV-001
        [HttpGet]
        public async Task<ActionResult> Ver(int id)
        {
            if (id==0)
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "Id requerido");

            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada");

            try
            {
                // 1) intentar traer desde la API
                var item = await _api.GetByIdAsync(id, token);

                

                // 3) historial: primero API
                var historial = await _api.GetMovimientosAsync(item.IdArticulo, token);

                // 4) si la API no devolvió historial, ponemos una lista en duro
                if (historial == null || historial.Count == 0)
                {
                    historial = new List<Front_Hoteleria.Dto.Inventario.InventarioMovimientoPostDto>
            {
                new Front_Hoteleria.Dto.Inventario.InventarioMovimientoPostDto
                {
                    // ajusta estos nombres a los reales de tu DTO
                    FechaMovimiento = DateTime.Today.AddDays(-2),
                    TipoMovimiento = "Verificación",
                    HabitacionDesde = "0002",
                    HabitacionHasta = "0002",
                    Responsable = "Admin",
                    Motivo = "Control rutinario del inventario"
                },
                new Front_Hoteleria.Dto.Inventario.InventarioMovimientoPostDto
                {
                    FechaMovimiento = DateTime.Today.AddDays(-10),
                    TipoMovimiento = "Traslado",
                    HabitacionDesde = "0001",
                    HabitacionHasta = "0002",
                    Responsable = "Bodega",
                    Motivo = "Reubicación del artículo"
                }
            };
                }

                ViewBag.Historial = historial;

                // 5) devolvemos el modal
                return PartialView("~/Views/Inventario/_VerInventario.cshtml", item);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("[InventarioController.Ver] " + ex);
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al obtener el artículo");
            }
        }

        // GET: /Inventario/RegistrarMovimiento
        [HttpGet]
        public ActionResult RegistrarMovimiento(string id)
        {
            // si quieres, aquí podrías traer la lista real de artículos desde tu servicio
            // y pasarla con ViewBag.Articulos
            ViewBag.ArticuloId = id;  // para preseleccionar

            return PartialView("~/Views/Inventario/_RegistrarMovimiento.cshtml");
        }

        // POST: /Inventario/RegistrarMovimiento
        [HttpPost]
        public ActionResult RegistrarMovimiento(InventarioMovimientoPostDto dto)
        {
            // aquí llamarías a tu API para registrar el movimiento
            // por ahora devolvemos OK
            return Json(new { ok = true });
        }
        [HttpGet]
        public ActionResult Importar()
        {
            return PartialView("~/Views/Inventario/_ImportarMasivo.cshtml");
        }

        // opcional: para que el JS de arriba funcione
        [HttpPost]
        public ActionResult Importar(HttpPostedFileBase Archivo, bool? Sobrescribir, bool? Validar)
        {
            // aquí procesas el Excel...
            // por ahora devolvemos OK
            return Json(new { ok = true });
        }


        // TABLA
        // GET: /Inventario/Tabla
        [HttpGet]
        [ActionName("Tabla")]
        public async Task<ActionResult> Tabla(string criterio, string categoria, string estado, string habitacion)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada");

                var lista = await _api.ListarAsync(criterio, categoria, estado, habitacion, token);

                // si la api no trae nada, metemos datos de demo para que veas diseño
                if (lista == null || !lista.Any())
                {
                    
                }

                // ~/Views/Inventario/_TablaInventario.cshtml
                return PartialView("~/Views/Inventario/_TablaInventario.cshtml", lista);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InventarioController.Tabla] " + ex);
                return new HttpStatusCodeResult(500, "Error al cargar inventario");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Upsert(int id = 0, bool soloLectura = false)
        {
            var token = GetBearer();
            InventarioItemDto dto = null;

            // si viene un id > 0 intento ir a la API
            if (id > 0 && !string.IsNullOrWhiteSpace(token))
            {
                dto = await _api.GetByIdAsync(id, token);
            }

            // si no hay API o no devolvió nada, maqueto
            if (dto == null)
            {
                dto = new InventarioItemDto
                {
                    IdArticulo = 0

                };
            }

            ViewBag.SoloLectura = soloLectura;
            return PartialView("~/Views/Inventario/_UpsertInventario.cshtml", dto);
        }

        [HttpPost]
        public async Task<ActionResult> Guardar(InventarioItemDto dto)
        {
            if (dto == null)
                return Json(new { ok = false, msg = "Datos vacíos" });

            var token = GetBearer();

            try
            {
                bool ok;
                if (dto.IdArticulo>0)
                    ok = await _api.ActualizarAsync(dto, token);
                else
                    ok = await _api.CrearAsync(dto, token);

                if (!ok)
                    return Json(new { ok = false, msg = "No se pudo guardar en la API" });

                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InventarioController.Guardar] " + ex);
                return Json(new { ok = false, msg = "Error inesperado al guardar" });
            }
        }


        [HttpPost]
        public async Task<ActionResult> Eliminar(int id)
        {
            var token = GetBearer();
            try
            {
                var ok = await _api.EliminarAsync(id, token);
                return Json(new { ok });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InventarioController.Eliminar] " + ex);
                return Json(new { ok = false, msg = "Error al eliminar" });
            }
        }
    }
}
