using Front_Hoteleria.Dto.Roles;
using Front_Hoteleria.Services.Roles;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class RolesController : Controller
    {
        private readonly IRolesService _api;

        public RolesController() : this(new RolesService()) { }

        public RolesController(IRolesService api)
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
                Trace.TraceError("[RolesController.GetBearer] " + ex);
                return null;
            }
        }

        // GET: /Roles
        [HttpGet]
        public ActionResult Index()
        {
            // vista principal: ~/Views/Roles/Index.cshtml
            return View("~/Views/Roles/Index.cshtml");
        }

        // ====== KPIs (panel superior) ======
        [HttpGet]
        public async Task<ActionResult> Resumen()
        {
            var token = GetBearer();
            var dto = await _api.ResumenAsync(token);

            if (dto == null)
            {
                dto = new RolesKpiDto
                {
                    TotalRoles = 5,
                    Administradores = 3,
                    Supervisores = 8,
                    Trabajadores = 145
                };
            }

            // ~/Views/Roles/_DashboardRoles.cshtml
            return PartialView("~/Views/Roles/_DashboardRoles.cshtml", dto);
        }

        // ====== TABLA / LISTA ======
        // GET: /Roles/Tabla?criterio=admin
        [HttpGet]
        public async Task<ActionResult> Tabla(string criterio)
        {
            try
            {
                var token = GetBearer();
                var lista = await _api.ListarAsync(criterio, token);

                // si la api no respondió, metemos datos demo
                if (lista == null || !lista.Any())
                {
                    lista = new List<RolDto>
                    {
                        new RolDto {
                            Id = 1,
                            Nombre = "Administrador",
                            Codigo = "ADMIN",
                            Descripcion = "Acceso completo a todas las funcionalidades",
                            UsuariosAsignados = 3,
                            Permisos = BuildDefaultPermisos()
                        },
                        new RolDto {
                            Id = 2,
                            Nombre = "Supervisor",
                            Codigo = "SUPERVISOR",
                            Descripcion = "Acceso de supervisión a operaciones diarias",
                            UsuariosAsignados = 8,
                            Permisos = BuildDefaultPermisos().Where(p => p.Codigo != "roles").ToList()
                        },
                        new RolDto {
                            Id = 3,
                            Nombre = "Trabajador",
                            Codigo = "WORKER",
                            Descripcion = "Acceso básico para operaciones",
                            UsuariosAsignados = 145,
                            Permisos = new List<RolPermisoDto>
                            {
                                new RolPermisoDto{ Codigo="rooms", Nombre="Gestión de Habitaciones", Habilitado=true },
                                new RolPermisoDto{ Codigo="services", Nombre="Gestión de Servicios", Habilitado=true },
                            }
                        }
                    };
                }

                // ~/Views/Roles/_TablaRoles.cshtml
                return PartialView("~/Views/Roles/_TablaRoles.cshtml", lista);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[RolesController.Tabla] " + ex);
                return new HttpStatusCodeResult(500, "Error al cargar roles");
            }
        }

        // ====== GET: crear (muestra modal) ======
        [HttpGet]
        public ActionResult Crear()
        {
            // acá simulas traer los permisos desde la API
            var dto = new RolDto
            {
                Permisos = BuildDefaultPermisos()
            };

            return PartialView("~/Views/Roles/_CrearRol.cshtml", dto);
        }

        // ====== GET: editar ======
        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            if (id==0)
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "Id requerido");

            var token = GetBearer();
            var dto = await _api.ObtenerPorIdAsync(id, token);

            if (dto == null)
            {
                // si no lo encontró, armamos uno en duro
                dto = new RolDto
                {
                    Id = id,
                    Nombre = "Rol Demo",
                    Codigo = "codigo",
                    Descripcion = "Rol de demostración",
                    Permisos = BuildDefaultPermisos()
                };
            }
            else
            {
                // por seguridad, si la API no mandó la lista completa de permisos,
                // la completamos con las que usamos en la UI
                dto.Permisos = MergePermisos(dto.Permisos, BuildDefaultPermisos());
            }

            return PartialView("~/Views/Roles/_CrearRol.cshtml", dto);
        }

        // ====== POST: guardar (crear o actualizar) ======
        [HttpPost]
        public async Task<ActionResult> Guardar(RolDto dto)
        {
            if (dto == null)
                return Json(new { ok = false, msg = "Datos vacíos" });

            var token = GetBearer();

            try
            {
                bool ok;
                if (dto.Id >0)
                    ok = await _api.ActualizarAsync(dto, token);
                else
                    ok = await _api.CrearAsync(dto, token);

                if (!ok)
                    return Json(new { ok = false, msg = "No se pudo guardar en la API" });

                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[RolesController.Guardar] " + ex);
                return Json(new { ok = false, msg = "Error inesperado al guardar" });
            }
        }

        // ====== GET: /Roles/Asignar  (muestra el modal) ======
        [HttpGet]
        public async Task<ActionResult> Asignar()
        {
            // si tuvieras API, aquí la llamas. Ahora: datos en duro

            // trabajadores de ejemplo
            var trabajadores = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Juan Pérez - Constructora ABC" },
        new SelectListItem { Value = "2", Text = "María González - Servicios Mineros" },
        new SelectListItem { Value = "3", Text = "Carlos Méndez - Contratista Sur" },
    };
            ViewBag.Trabajadores = trabajadores;

            // roles de ejemplo (los mismos que usas en la tabla)
            var roles = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Administrador (ADMIN)" },
        new SelectListItem { Value = "2", Text = "Supervisor (SUPERVISOR)" },
        new SelectListItem { Value = "3", Text = "Trabajador (WORKER)" },
        new SelectListItem { Value = "4", Text = "Invitado (GUEST)" },
        new SelectListItem { Value = "5", Text = "Supervisor de Mantenimiento (SUP_MANT)" },
    };
            ViewBag.Roles = roles;

            var dto = new AsignacionRolDto
            {
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddMonths(1)
            };

            return PartialView("~/Views/Roles/_AsignacionesRoles.cshtml", dto);
        }

        // ====== POST: /Roles/Asignar  (guarda la asignación) ======
        [HttpPost]
        public async Task<ActionResult> Asignar(AsignacionRolDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.TrabajadorId) || dto.RolId == 0)
                return Json(new { ok = false, msg = "Complete trabajador y rol." });

            try
            {
                // aquí llamarías al servicio real:
                // var token = GetBearer();
                // var okApi = await _api.AsignarRolAsync(dto, token);

                // demo: siempre ok
                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[RolesController.Asignar POST] " + ex);
                return Json(new { ok = false, msg = "Error al asignar el rol." });
            }
        }

        [HttpGet]
        public ActionResult MatrizPermisos()
        {
            return PartialView("~/Views/Roles/_MatrizPermisos.cshtml");
        }


        // ====== POST: eliminar ======
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
                Trace.TraceError("[RolesController.Eliminar] " + ex);
                return Json(new { ok = false, msg = "Error al eliminar" });
            }
        }

        // ===== helpers internos =====
        private static List<RolPermisoDto> BuildDefaultPermisos()
        {
            return new List<RolPermisoDto>
            {
                new RolPermisoDto{ Codigo = "rooms",       Nombre = "Gestión de Habitaciones",      Habilitado = true },
                new RolPermisoDto{ Codigo = "reservations",Nombre = "Gestión de Reservas",          Habilitado = true },
                new RolPermisoDto{ Codigo = "services",    Nombre = "Gestión de Servicios",         Habilitado = true },
                new RolPermisoDto{ Codigo = "camps",       Nombre = "Gestión de Campamentos",       Habilitado = true },
                new RolPermisoDto{ Codigo = "contracts",   Nombre = "Gestión de Contratos",         Habilitado = true },
                new RolPermisoDto{ Codigo = "staff",       Nombre = "Gestión de Dotaciones",        Habilitado = true },
                new RolPermisoDto{ Codigo = "roles",       Nombre = "Gestión de Roles",             Habilitado = true },
                new RolPermisoDto{ Codigo = "reports",     Nombre = "Reportes y Estadísticas",      Habilitado = true },
            };
        }

        // completa permisos que vengan de la API con los que necesita la UI
        private static List<RolPermisoDto> MergePermisos(List<RolPermisoDto> actuales, List<RolPermisoDto> basePermisos)
        {
            if (actuales == null || actuales.Count == 0)
                return basePermisos;

            var dict = actuales.ToDictionary(p => p.Codigo, p => p, StringComparer.OrdinalIgnoreCase);

            foreach (var p in basePermisos)
            {
                if (!dict.ContainsKey(p.Codigo))
                    actuales.Add(new RolPermisoDto
                    {
                        Codigo = p.Codigo,
                        Nombre = p.Nombre,
                        Habilitado = false
                    });
            }

            return actuales;
        }
    }
}
