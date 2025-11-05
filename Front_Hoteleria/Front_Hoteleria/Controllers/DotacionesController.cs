using Front_Hoteleria.Dto.Dotaciones;
using Front_Hoteleria.Services.Dotaciones;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class DotacionesController : Controller
    {
        private readonly IDotacionesService _api;

        public DotacionesController() : this(new DotacionesService()) { }

        public DotacionesController(IDotacionesService api)
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
                Trace.TraceError("[DotacionesController.GetBearer] " + ex);
                return null;
            }
        }

        // ================== VISTA PRINCIPAL ==================
        [HttpGet]
        public ActionResult Index()
        {
            return View("~/Views/Dotaciones/Index.cshtml");
        }

        // ================== DASHBOARD (lo que carga #dashDotContainer) ==================
        // el Index llama a: @Url.Action("Dashboard","Dotaciones")
        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            var token = GetBearer();

            // si no hay token, igual devolvemos algo vacío para que no reviente el .load()
            DotacionKPIDto dto = new DotacionKPIDto();
            //{
            //    TotalTrabajadores = 0,
            //    TurnoDia = 0,
            //    TurnoNoche = 0,
            //    FueraServicio = 0
            //};

            try
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var desdeApi = await _api.ResumenAsync(token);
                    if (desdeApi != null)
                        dto = desdeApi;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[DotacionesController.Dashboard] " + ex);
            }

            return PartialView("~/Views/Dotaciones/_DashboardDotaciones.cshtml", dto);
        }

        // ================== TABLA (lo que carga #tablaDotContainer) ==================
        // el Index llama con GET /Dotaciones/Tabla?criterio=xxx
        [HttpGet]
       
        public async Task<ActionResult> Tabla(int? empresaId, string criterio)
        {
            List<DotacionDto> lista = null;

            try
            {
                var token = GetBearer();

                // 1) Intentamos llamar a la API SOLO si hay token
                if (!string.IsNullOrWhiteSpace(token))
                {
                    lista = await _api.ListarAsync(empresaId, criterio, token);
                }
            }
            catch (Exception exApi)
            {
                // si la API falló, lo registramos y seguimos con datos en duro
                Trace.TraceError("[DotacionesController.Tabla] error API: " + exApi);
            }

            // 2) Si la API no trajo nada, usamos datos de prueba
            if (lista == null || lista.Count == 0)
            {
                lista = new List<DotacionDto>
        {
            // Empresa 1
            new DotacionDto
            {
                IdDotacion = 1,
                IdEmpresa = 1,
                Empresa = "Constructora ABC Ltda.",
                RutEmpresa = "12.345.678-9",
                Nombre = "Juan",
                Apellido = "Pérez",
                Rut = "12.345.678-9",
                Cargo = "Supervisor",
                TurnoCodigo = "day",
                TurnoNombre = "Día"
            },
            new DotacionDto
            {
                IdDotacion = 2,
                IdEmpresa = 1,
                Empresa = "Constructora ABC Ltda.",
                RutEmpresa = "12.345.678-9",
                Nombre = "Carlos",
                Apellido = "Méndez",
                Rut = "11.222.333-4",
                Cargo = "Operador",
                TurnoCodigo = "night",
                TurnoNombre = "Noche"
            },

            // Empresa 2
            new DotacionDto
            {
                IdDotacion = 3,
                IdEmpresa = 2,
                Empresa = "Servicios Mineros XYZ S.A.",
                RutEmpresa = "98.765.432-1",
                Nombre = "María",
                Apellido = "González",
                Rut = "13.456.789-0",
                Cargo = "Gerente",
                TurnoCodigo = "maintenance",
                TurnoNombre = "Mantenimiento"
            }
        };
            }

            // 3) Filtro por empresa (vale tanto para datos reales como de prueba)
            if (empresaId.HasValue)
            {
                lista = lista.Where(x => x.IdEmpresa == empresaId.Value).ToList();
            }

            // 4) Filtro de texto
            if (!string.IsNullOrWhiteSpace(criterio))
            {
                var f = criterio.ToLower().Trim();
                lista = lista
                    .Where(x =>
                        (!string.IsNullOrWhiteSpace(x.Nombre) && x.Nombre.ToLower().Contains(f)) ||
                        (!string.IsNullOrWhiteSpace(x.Apellido) && x.Apellido.ToLower().Contains(f)) ||
                        (!string.IsNullOrWhiteSpace(x.Rut) && x.Rut.ToLower().Contains(f)) ||
                        (!string.IsNullOrWhiteSpace(x.Empresa) && x.Empresa.ToLower().Contains(f)))
                    .ToList();
            }

            // 5) devolvemos el parcial con el diseño de tarjetas
            return PartialView("~/Views/Dotaciones/_TablaDotaciones.cshtml", lista);
        }


        // ================== MODAL: ALTA / EDICIÓN ==================
        // el Index hace $.get(urlUpsertDot, ...)
        [HttpGet]
        public async Task<ActionResult> Upsert(int? id)
        {
            var token = GetBearer();
            DotacionDto model = new DotacionDto();

            if (id.HasValue && id.Value > 0)
            {
                try
                {
                    var dto = await _api.ObtenerPorIdAsync(id.Value, token);
                    if (dto != null)
                        model = dto;
                }
                catch (Exception ex)
                {
                    Trace.TraceError("[DotacionesController.Upsert] " + ex);
                }
            }

            return PartialView("~/Views/Dotaciones/_UpsertDotaciones.cshtml", model);
        }

        // ================== MODAL: CARGA MASIVA ==================
        // el Index hace $.get(urlCargaDot, ...)
        [HttpGet]
        public ActionResult CargaMasiva()
        {
            return PartialView("~/Views/Dotaciones/_CargaMasivaDotacion.cshtml");
        }

        // ================== MODAL: TURNOS ==================
        // el Index hace $.get(urlTurnosDot, ...)
        [HttpGet]
        public ActionResult Turnos()
        {
            return PartialView("~/Views/Dotaciones/_TurnosDotacion.cshtml");
        }

        // opcional: guardar (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Guardar(DotacionDto dto)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            var ok = false;

            try
            {
                if (dto.IdDotacion == 0)
                    ok = await _api.CrearAsync(dto, token);
                else
                    ok = await _api.ModificarAsync(dto, token);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[DotacionesController.Guardar] " + ex);
            }

            return Json(new { ok, message = ok ? "Dotación guardada." : "No se pudo guardar." });
        }
    }
}
