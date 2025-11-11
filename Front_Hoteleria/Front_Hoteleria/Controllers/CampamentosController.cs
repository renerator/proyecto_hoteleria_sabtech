using Front_Hoteleria.Dto.Campamentos;
using Front_Hoteleria.Services.Campamentos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class CampamentosController : Controller
    {
        private readonly ICampamentosService _api;

        public CampamentosController() : this(new CampamentosService()) { }

        public CampamentosController(ICampamentosService api)
        {
            _api = api;
        }

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
            return View("~/Views/Campamentos/Index.cshtml");
        }

        // ===== KPI =====
        [HttpGet]
        public async Task<ActionResult> Resumen()
        {
            var token = GetBearer();
            var dto = await _api.ResumenAsync(token);

            return PartialView("~/Views/Campamentos/_DashboardCampamentos.cshtml", dto);
        }

        // ===== TABLA (lista) =====
        [HttpGet]
        public async Task<ActionResult> Tabla(string criterio, string estado)
        {
            try
            {
                var token = GetBearer();
                var lista = await _api.ListarAsync(criterio, estado, token);

                // 1) si la API no devolvió nada, metemos datos de demo
               

                // 2) filtramos por criterio (nombre, código o ubicación)
                if (!string.IsNullOrWhiteSpace(criterio))
                {
                    criterio = criterio.Trim().ToLower();
                    lista = lista
                        .Where(c =>
                            (c.Nombre ?? "").ToLower().Contains(criterio) ||
                            (c.Codigo ?? "").ToLower().Contains(criterio) ||
                            (c.Ubicacion ?? "").ToLower().Contains(criterio))
                        .ToList();
                }

                // 3) filtramos por estado si viene
                if (!string.IsNullOrWhiteSpace(estado))
                {
                    estado = estado.Trim().ToLower();
                    lista = lista
                        .Where(c => (c.Estado ?? "").ToLower() == estado)
                        .ToList();
                }

                return PartialView("~/Views/Campamentos/_TablaCampamentos.cshtml", lista);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CampamentosController.Tabla] " + ex);
                return new HttpStatusCodeResult(500, "Error al cargar campamentos");
            }
        }


        // ===== FORM (modal) =====
        [HttpGet]
        public async Task<ActionResult> Upsert(int IdCampamento, bool soloLectura = false)
        {
            var token = GetBearer();
            CampamentoDto dto = null;

            if (IdCampamento > 0)
            {
                dto = await _api.ObtenerPorIdAsync(IdCampamento, token);
            }

            if (dto == null)
            {
                dto = new CampamentoDto
                {
                    IdCampamento = IdCampamento,
                    Estado = "active",
                    Capacidad = 1,
                    OcupacionActual = 0
                };
            }

            ViewBag.SoloLectura = soloLectura;
            return PartialView("~/Views/Campamentos/_UpsertCampamentos.cshtml", dto);
        }


        [HttpPost]
        public async Task<ActionResult> Guardar(CampamentoDto dto)
        {
            var token = GetBearer();
            if (dto == null)
                return Json(new { ok = false, msg = "Datos vacíos" });

            

            try
            {
                bool ok;
                if (dto.IdCampamento==0)
                    ok = await _api.CrearAsync(dto, token);
                else
                    ok = await _api.ActualizarAsync(dto, token);

                if (!ok)
                    ok = true; // maqueta

                return Json(new { ok });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CampamentosController.Guardar] " + ex);
                return Json(new { ok = false, msg = "Error inesperado al guardar" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Eliminar(int IdCampamento)
        {
            var token = GetBearer();
            try
            {
                var ok = await _api.EliminarAsync(IdCampamento, token);
                if (!ok) ok = true; // maqueta
                return Json(new { ok });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CampamentosController.Eliminar] " + ex);
                return Json(new { ok = false, msg = "Error al eliminar" });
            }
        }
    }
}
