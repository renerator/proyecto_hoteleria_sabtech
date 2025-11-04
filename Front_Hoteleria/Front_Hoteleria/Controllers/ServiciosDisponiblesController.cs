using Front_Hoteleria.Dto.Servicio;
using Front_Hoteleria.Services.Servicio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class ServiciosDisponiblesController : Controller
    {
        private readonly IServicioService _api;

        public ServiciosDisponiblesController() : this(new ServicioService()) { }

        public ServiciosDisponiblesController(IServicioService api)
        {
            _api = api;
        }

        // ===== helper token =====
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
                Trace.TraceError($"[GetBearer] {ex}");
                return null;
            }
        }

        // ===== vista principal =====
        [HttpGet]
        public ActionResult Index()
        {
            return View("~/Views/ServiciosDisponibles/Index.cshtml");
        }

        // ===== partial KPIs =====
        [HttpGet]
        public async Task<ActionResult> Paneles()
        {
            var kpi = new ServicioKpiDto();
            try
            {
                var token = GetBearer();
                var apiKpi = await _api.KpiServiciosAsync(token);
                if (apiKpi != null) kpi = apiKpi;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Paneles] {ex}");
            }
            return PartialView("~/Views/ServiciosDisponibles/_Paneles.cshtml", kpi);
        }

        [HttpGet]
        public ActionResult ImportarMasivo()
        {
            // si quieres validar sesión, puedes hacer lo mismo que en los otros métodos:
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult(401, "Sesión expirada.");

            return PartialView("~/Views/ServiciosDisponibles/_ImportarMasivo.cshtml");
        }

        [HttpGet]
        public async Task<ActionResult> Ver(int id)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult(401, "Sesión expirada.");

            // usamos tu método que trae por id (el que devolvía List<ServicioDto>)
            var lista = await _api.VerificaServicioPorId(
                new ServicioDto { IdServicio = id },
                token
            );

            var model = lista?.FirstOrDefault();
            if (model == null)
            {
                // si no vino nada, mandamos un dto mínimo para que no rompa la vista
                model = new ServicioDto
                {
                    IdServicio = id,
                    NombreServicio = "(no encontrado)",
                    Estado = false
                };
            }

            return PartialView("~/Views/ServiciosDisponibles/_DetalleServicio.cshtml", model);
        }

        [HttpGet]
        public ActionResult ConfigurarServicios()
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult(401, "Sesión expirada.");

            // si en el futuro querés traer la config desde el API, acá la cargas
            return PartialView("~/Views/ServiciosDisponibles/_ConfigurarServicios.cshtml");
        }

        // ===== crear (GET) -> se llama desde botón "Agregar Servicio" =====
        [HttpGet]
        public ActionResult Crear()
        {
            var model = new ServicioDto
            {
                IdServicio = 0,
                Estado = true
            };
            // mismo formulario que usas para editar
            return PartialView("~/Views/ServiciosDisponibles/_Upsert.cshtml", model);
        }

        // ===== crear / modificar (POST) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Guardar(ServicioDto dto)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            // valores por defecto que espera el API
            dto.IdEmpresa = dto.IdEmpresa == 0 ? 1 : dto.IdEmpresa;
            dto.IdTipoServicio = dto.IdTipoServicio == 0 ? 1 : dto.IdTipoServicio;
            dto.Estado = true; // como es bool, lo seteamos directo

            bool ok;
            string mensaje;

            if (dto.IdServicio == 0)
            {
                // crear
                ok = await _api.CrearServicioAsync(dto, token);
                mensaje = ok ? "Servicio creado." : "No se pudo crear.";
            }
            else
            {
                // modificar
                ok = await _api.ModificarServicioAsync(dto, token);
                mensaje = ok ? "Servicio actualizado." : "No se pudo actualizar.";
            }

            return Json(new { ok, message = mensaje });
        }


        // ===== modificar (GET) -> se llama desde botón Editar =====
        [HttpGet]
        public async Task<ActionResult> Modificar(int id)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." }, JsonRequestBehavior.AllowGet);

            // llamamos al servicio que acabas de implementar
            var lista = await _api.VerificaServicioPorId(
                new ServicioDto { IdServicio = id },
                token
            );

            // tomamos el primero (tu API devuelve lista)
            var model = lista?.FirstOrDefault();

            if (model == null)
            {
                // si no vino nada, igual mandamos un dto con el id para no romper la vista
                model = new ServicioDto
                {
                    IdServicio = id,
                    Estado = true
                };
            }

            return PartialView("~/Views/ServiciosDisponibles/_Upsert.cshtml", model);
        }


       

        // ===== eliminar (POST) -> se llama desde botón Eliminar =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Eliminar(int idServicio)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            var ok = await _api.EliminarServicioAsync(idServicio, token);
            return Json(new { ok, message = ok ? "Servicio eliminado." : "No se pudo eliminar." });
        }

        // ===== tabla =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Tabla(int? categoria, int? estado, int? prioridad, string criterio)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

            
            var lista = await _api.ListarServiciosAsync(estado, token) ?? new List<ServicioDto>();

            if (categoria.HasValue)
                lista = lista.Where(x => x.IdServiciosCategoria == categoria.Value).ToList();

            if (prioridad.HasValue)
                lista = lista.Where(x => x.IdServicioPrioridad == prioridad.Value).ToList();

            if (!string.IsNullOrWhiteSpace(criterio))
            {
                var crit = criterio.ToLower().Trim();
                lista = lista.Where(x =>
                    (!string.IsNullOrEmpty(x.NombreServicio) && x.NombreServicio.ToLower().Contains(crit)) ||
                    (!string.IsNullOrEmpty(x.NombreCategoria) && x.NombreCategoria.ToLower().Contains(crit)) ||
                    (!string.IsNullOrEmpty(x.NombrePrioridad) && x.NombrePrioridad.ToLower().Contains(crit))
                ).ToList();
            }

            return PartialView("~/Views/ServiciosDisponibles/_TablaServicioDisponibles.cshtml", lista);
        }

        // ===== combos =====
        [HttpGet]
        public async Task<ActionResult> CategoriaCombo(int vigencia = 1)
        {
            var token = GetBearer();
            var lista = await _api.ListarServiciosCategoriaAsync(vigencia, token);
            var data = lista.Select(x => new { value = x.IdServiciosCategoria, text = x.Descripcion });
            return Json(new { ok = true, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> EstadoCombo(int vigencia = 1)
        {
            var token = GetBearer();
            var lista = await _api.ListarServicioEstadoAsync(vigencia, token);
            var data = lista.Select(x => new { value = x.IdServicioEstado, text = x.Descripcion });
            return Json(new { ok = true, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> PrioridadCombo(int vigencia = 1)
        {
            var token = GetBearer();
            var lista = await _api.ListarServicioPrioridadAsync(vigencia, token);
            var data = lista.Select(x => new { value = x.idServicioPrioridad, text = x.Descripcion });
            return Json(new { ok = true, data }, JsonRequestBehavior.AllowGet);
        }
    }
}
