using Front_Hoteleria.Dto.Servicio;
using Front_Hoteleria.Services.Servicio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class ServiciosDisponiblesController : Controller
    {
        private readonly IServicioService _api;

        public ServiciosDisponiblesController()
            : this(new ServicioService())
        { }

        public ServiciosDisponiblesController(IServicioService api)
        {
            _api = api;
        }

        // ================= helpers =================
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

        // ================= combos =================

        [HttpGet]
        public async Task<ActionResult> CategoriaCombo(int vigencia = 1)
        {
            try
            {
                var token = GetBearer();
                var lista = await _api.ListarServiciosCategoriaAsync(vigencia, token);

                var data = lista.Select(x => new
                {
                    value = x.IdServiciosCategoria,
                    text = x.Descripcion,
                    id = x.IdServiciosCategoria
                });

                return Json(new { ok = true, data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> EstadoCombo(int vigencia = 1)
        {
            try
            {
                var token = GetBearer();
                var lista = await _api.ListarServicioEstadoAsync(vigencia, token);

                var data = lista.Select(x => new
                {
                    value = x.IdServicioEstado,
                    text = x.Descripcion,
                    id = x.IdServicioEstado
                });

                return Json(new { ok = true, data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> PrioridadCombo(int vigencia = 1)
        {
            try
            {
                var token = GetBearer();
                var lista = await _api.ListarServicioPrioridadAsync(vigencia, token);

                var data = lista.Select(x => new
                {
                    value = x.idServicioPrioridad,
                    text = x.Descripcion,
                    id = x.idServicioPrioridad
                });

                return Json(new { ok = true, data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ================= vistas =================

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Servicios";
            return View("~/Views/ServiciosDisponibles/Index.cshtml");
        }

        // esta la llama el index por AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Tabla(
            int? categoria,
            int? estado,
            int? prioridad,
            string criterio
        )
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

            // tu API lista por estado (vigencia)
            var lista = await _api.ListarServiciosAsync(estado, token) ?? new List<ServicioDto>();

            // filtros en front
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

        [HttpGet]
        public ActionResult Upsert(int? id)
        {
            var model = new ServicioDto
            {
                IdServicio = id ?? 0,
                Estado = true
            };
           
            return PartialView("~/Views/ServiciosDisponibles/_Upsert.cshtml", model);
        }
        [HttpGet]
        public async Task<ActionResult> Kpi()
        {
            var token = GetBearer();
            var kpi = await _api.KpiServiciosAsync(token);
            return PartialView("~/Views/ServiciosDisponibles/_PanelesServicioDisponibles.cshtml", kpi);
        }
        // ================= guardar desde el modal =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Guardar(ServicioDto dto)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            // valores que el modal no envía pero el backend sí espera
            if (dto.IdEmpresa == 0) dto.IdEmpresa = 1;        // puedes cambiar 1 por el que corresponda
            if (dto.IdTipoServicio == 0) dto.IdTipoServicio = 1;

            bool ok;

            if (dto.IdServicio > 0)
            {
                // editar
                ok = await _api.ModificarServicioAsync(dto, token);
                return Json(new { ok, message = ok ? "Servicio actualizado." : "No se pudo actualizar." });
            }
            else
            {
                // crear
                ok = await _api.CrearServicioAsync(dto, token);
                return Json(new { ok, message = ok ? "Servicio creado." : "No se pudo crear." });
            }
        }

        // ================= eliminar =================

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
    }
}
