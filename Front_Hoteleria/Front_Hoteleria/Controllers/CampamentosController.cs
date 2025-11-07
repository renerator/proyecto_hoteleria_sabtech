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
                    ?? (Request.Cookies["access_token"] != null
                        ? Request.Cookies["access_token"].Value
                        : null);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CampamentosController.GetBearer] " + ex);
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

            if (dto == null)
            {
                dto = new CampamentoKpiDto
                {
                    CampamentosActivos= 22,
                    AreasComunes =22,
                    Habitaciones=1,
                   TasaUtilizacion =6
                 };
            }

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
                if (lista == null || !lista.Any())
                {
                    lista = new List<CampamentoDto>
            {
                new CampamentoDto
                {
                    Id = "CAMP-001",
                    Nombre = "Campamento Norte",
                    Codigo = "CAMP-N-001",
                    Ubicacion = "Sector Norte, Mina Escondida",
                    Encargado = "Juan Pérez",
                    Capacidad = 200,
                    OcupacionActual = 156,
                    Estado = "active",
                    Areas = new List<CampamentoAreaDto>
                    {
                        new CampamentoAreaDto { Nombre = "Comedor Principal", Capacidad = 100, Estado = "active" },
                        new CampamentoAreaDto { Nombre = "Lavandería Central", Capacidad = 50, Estado = "active" },
                        new CampamentoAreaDto { Nombre = "Sala de Recreación", Capacidad = 30, Estado = "maintenance" },
                    }
                },
                new CampamentoDto
                {
                    Id = "CAMP-002",
                    Nombre = "Campamento Sur",
                    Codigo = "CAMP-S-002",
                    Ubicacion = "Sector Sur, Mina Los Pelambres",
                    Encargado = "María González",
                    Capacidad = 150,
                    OcupacionActual = 120,
                    Estado = "active",
                    Areas = new List<CampamentoAreaDto>
                    {
                        new CampamentoAreaDto { Nombre = "Comedor Sur", Capacidad = 80, Estado = "active" },
                        new CampamentoAreaDto { Nombre = "Gimnasio", Capacidad = 20, Estado = "active" }
                    }
                },
                new CampamentoDto
                {
                    Id = "CAMP-003",
                    Nombre = "Campamento Mantenimiento",
                    Codigo = "CAMP-M-003",
                    Ubicacion = "Planta Central",
                    Encargado = "Pedro Silva",
                    Capacidad = 80,
                    OcupacionActual = 35,
                    Estado = "maintenance",
                    Areas = new List<CampamentoAreaDto>
                    {
                        new CampamentoAreaDto { Nombre = "Taller", Capacidad = 15, Estado = "active" }
                    }
                },
                new CampamentoDto
                {
                    Id = "CAMP-004",
                    Nombre = "Campamento Antiguo",
                    Codigo = "CAMP-A-004",
                    Ubicacion = "Sector Antiguo",
                    Encargado = "Sin asignar",
                    Capacidad = 60,
                    OcupacionActual = 0,
                    Estado = "inactive",
                    Areas = new List<CampamentoAreaDto>()
                }
            };
                }

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
        public async Task<ActionResult> Upsert(string id)
        {
            var token = GetBearer();
            CampamentoDto dto = null;

            if (!string.IsNullOrWhiteSpace(id))
            {
                dto = await _api.ObtenerPorIdAsync(id, token);
            }

            if (dto == null)
            {
                dto = new CampamentoDto
                {
                    Id = id,
                    Estado = "active",
                    Capacidad = 200,
                    OcupacionActual = 0
                };
            }

            return PartialView("~/Views/Campamentos/_UpsertCampamentos.cshtml", dto);
        }

        [HttpPost]
        public async Task<ActionResult> Guardar(CampamentoDto dto)
        {
            if (dto == null)
                return Json(new { ok = false, msg = "Datos vacíos" });

            var token = GetBearer();

            try
            {
                bool ok;
                if (string.IsNullOrWhiteSpace(dto.Id))
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
        public async Task<ActionResult> Eliminar(string id)
        {
            var token = GetBearer();
            try
            {
                var ok = await _api.EliminarAsync(id, token);
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
