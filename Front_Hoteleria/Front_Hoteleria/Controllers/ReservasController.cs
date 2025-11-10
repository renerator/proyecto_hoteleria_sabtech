using Front_Hoteleria.Dto.Reserva;

using Front_Hoteleria.Services.Reservas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IReservaService _api;

        public ReservasController() : this(new ReservaService()) { }

        public ReservasController(IReservaService api)
        {
            _api = api;
        }

        // =========================================================
        // helper para token
        // =========================================================
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
                Trace.TraceError("[ReservasController.GetBearer] " + ex);
                return null;
            }
        }

        // =========================================================
        // GET: /Reservas
        // vista principal
        // =========================================================
        [HttpGet]
        public ActionResult Index()
        {
            return View("~/Views/Reservas/Index.cshtml");
        }

        // =========================================================
        // GET: /Reservas/Dashboard
        // devuelve la parcial con los 4 KPIs
        // =========================================================
        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            var token = GetBearer();
            var dto = await _api.ResumenAsync(token);

            // fallback demo
            if (dto == null)
            {
                dto = new ReservaKPIDto
                {
                    Pendientes = 5,
                    Confirmadas = 18,
                    Rechazadas = 2,
                    Total = 25
                };
            }

            return PartialView("~/Views/Reservas/_DashboardReserva.cshtml", dto);
        }

        // =========================================================
        // POST: /Reservas/TablaPartial
        // la tabla principal (con filtros de fecha / estado / habitación)
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TablaPartial()
        {
            DateTime? fechaDesde = null;
            DateTime? fechaHasta = null;

            var strDesde = Request["fechaDesde"];
            var strHasta = Request["fechaHasta"];
            var estado = Request["estado"];
            var habitacion = Request["habitacion"];

            if (!string.IsNullOrWhiteSpace(strDesde) &&
                DateTime.TryParse(strDesde, out var d1))
                fechaDesde = d1;

            if (!string.IsNullOrWhiteSpace(strHasta) &&
                DateTime.TryParse(strHasta, out var d2))
                fechaHasta = d2;

            try
            {
                var token = GetBearer();
                var lista = await _api.ListarAsync(
                    estado: estado,
                    habitacion: habitacion,
                    fechaDesde: fechaDesde,
                    fechaHasta: fechaHasta,
                    bearer: token
                );

                // 👇 si la API no devuelve nada, metemos dummy
                if (lista == null || !lista.Any())
                {
                    lista = new List<ReservaDto>
            {
                new ReservaDto{
                    Codigo = "RES-006",
                    Id = "RES-006",
                    FechaEntrada = DateTime.Today.AddDays(-3),
                    FechaSalida  = DateTime.Today.AddDays(-2),
                    HuespedNombre = "Sofía Torres",
                    TipoHabitacionNombre = "Doble",
                    CantidadPersonas = 2,
                    Estado = "pendiente"
                },
                new ReservaDto{
                    Codigo = "RES-001",
                    Id = "RES-001",
                    FechaEntrada = DateTime.Today.AddDays(-2),
                    FechaSalida  = DateTime.Today.AddDays(1),
                    HuespedNombre = "Juan Pérez",
                    TipoHabitacionNombre = "Suite",
                    CantidadPersonas = 2,
                    Estado = "pendiente"
                },
                new ReservaDto{
                    Codigo = "RES-002",
                    Id = "RES-002",
                    FechaEntrada = DateTime.Today.AddDays(-1),
                    FechaSalida  = DateTime.Today.AddDays(3),
                    HuespedNombre = "María González",
                    TipoHabitacionNombre = "Doble",
                    CantidadPersonas = 2,
                    Estado = "pendiente"
                },
                new ReservaDto{
                    Codigo = "RES-003",
                    Id = "RES-003",
                    FechaEntrada = DateTime.Today,
                    FechaSalida  = DateTime.Today.AddDays(2),
                    HuespedNombre = "Carlos Rodríguez",
                    TipoHabitacionNombre = "Individual",
                    CantidadPersonas = 1,
                    Estado = "pendiente"
                },
                new ReservaDto{
                    Codigo = "RES-004",
                    Id = "RES-004",
                    FechaEntrada = DateTime.Today.AddDays(1),
                    FechaSalida  = DateTime.Today.AddDays(5),
                    HuespedNombre = "Ana Martínez",
                    TipoHabitacionNombre = "Familiar",
                    CantidadPersonas = 4,
                    Estado = "pendiente"
                },
                new ReservaDto{
                    Codigo = "RES-005",
                    Id = "RES-005",
                    FechaEntrada = DateTime.Today.AddDays(2),
                    FechaSalida  = DateTime.Today.AddDays(4),
                    HuespedNombre = "Luis Fernández",
                    TipoHabitacionNombre = "Suite",
                    CantidadPersonas = 2,
                    Estado = "pendiente"
                }
            };
                }

                return PartialView("~/Views/Reservas/_TablaReserva.cshtml", lista);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasController.TablaPartial] " + ex);
                return new HttpStatusCodeResult(500, "No se pudo cargar el listado de reservas");
            }
        }
        // GET: /Reservas/Rechazar?id=RES-006
        [HttpGet]
        public ActionResult Rechazar(string id)
        {
            // si no tengo la reserva real, armo una de demo como en tus capturas
            var dto = new ReservaDto
            {
                Id = string.IsNullOrWhiteSpace(id) ? "RES-006" : id,
                HuespedNombre = "Sofía Torres",
                HuespedEmail = "sofia.t@email.com"
            };

            return PartialView("~/Views/Reservas/_RechazarReserva.cshtml", dto);
        }

        // POST: /Reservas/Rechazar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rechazar(ReservaRechazoDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.IdReserva))
                return Json(new { ok = false, msg = "Id de reserva requerido." });

            // aquí iría el llamado real a tu API para rechazar la reserva
            // var token = GetBearer();
            // await _api.RechazarAsync(...)

            // por ahora devolvemos ok
            return Json(new { ok = true });
        }
        // =========================================================
        // GET: /Reservas/Upsert
        // abre el modal de crear/editar
        // =========================================================
        [HttpGet]
        public async Task<ActionResult> Upsert(string id = null)
        {
            ReservaDto dto = null;

            if (!string.IsNullOrWhiteSpace(id))
            {
                var token = GetBearer();
                dto = await _api.ObtenerPorIdAsync(id, token);
            }

            // si es nuevo o la API no devolvió nada, armamos dto vacío
            if (dto == null)
            {
                dto = new ReservaDto
                {
                    FechaEntrada = DateTime.Today.AddDays(1),
                    FechaSalida = DateTime.Today.AddDays(2),
                    CantidadPersonas = 1,
                    Estado = "pendiente"
                };
            }

            return PartialView("~/Views/Reservas/_UpsertReserva.cshtml", dto);
        }

        // =========================================================
        // POST: /Reservas/Upsert
        // guarda crear/editar
        // el JS que tienes espera:
        //   - JSON {ok:true} si salió bien
        //   - HTML parcial si hubo error de validación
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upsert(ReservaDto dto)
        {
            if (!ModelState.IsValid)
            {
                // devolvemos la misma parcial con los mensajes
                return PartialView("~/Views/Reservas/_UpsertReserva.cshtml", dto);
            }

            var token = GetBearer();

            try
            {
                bool ok;
                if (string.IsNullOrWhiteSpace(dto.Id))
                    ok = await _api.CrearAsync(dto, token);
                else
                    ok = await _api.ActualizarAsync(dto, token);

                if (ok)
                    return Json(new { ok = true });

                // si la API respondió 400/500, mostramos de nuevo el form
                ModelState.AddModelError("", "No se pudo guardar la reserva en la API.");
                return PartialView("~/Views/Reservas/_UpsertReserva.cshtml", dto);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasController.Upsert POST] " + ex);
                ModelState.AddModelError("", "Error inesperado al guardar la reserva.");
                return PartialView("~/Views/Reservas/_UpsertReserva.cshtml", dto);
            }
        }

        // =========================================================
        // POST: /Reservas/Eliminar
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Eliminar(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Json(new { ok = false, msg = "Id requerido" });

            var token = GetBearer();
            try
            {
                var ok = await _api.EliminarAsync(id, token);
                if (!ok)
                    return Json(new { ok = false, msg = "No se pudo eliminar en la API." });

                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasController.Eliminar] " + ex);
                return Json(new { ok = false, msg = "Error al eliminar la reserva." });
            }
        }

        // =========================================================
        // =============== C O M B O S =============================
        // Los tres de tu JS:
        //  - EstadoReservaCombo
        //  - HabitacionesCombo
        //  - TipoHabitacionesCombo
        // devuelven {ok:true, data:[{id, value, text}]}
        // =========================================================

        [HttpGet]
        public async Task<ActionResult> EstadoReservaCombo()
        {
            var token = GetBearer();
            var data = await _api.EstadosAsync(token) ?? new List<ComboItemDto>();

            // el js espera value/text
            var resp = data.Select(d => new
            {
                id = d.Id,
                value = d.Value,
                text = d.Text
            });

            return Json(new { ok = true, data = resp }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> HabitacionesCombo()
        {
            var token = GetBearer();
            var data = await _api.HabitacionesAsync(token) ?? new List<ComboItemDto>();

            var resp = data.Select(d => new
            {
                id = d.Id,
                value = d.Value,
                text = d.Text
            });

            return Json(new { ok = true, data = resp }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Asignar(string id)
        {
            // 1) demo de la reserva (si no vino id, armamos una)
            var reserva = new ReservaDto
            {
                Id = string.IsNullOrWhiteSpace(id) ? "RES-006" : id,
                HuespedNombre = "Sofía Torres",
                HuespedEmail = "sofia.t@email.com",
                FechaEntrada = new System.DateTime(2025, 11, 4),
                FechaSalida = new System.DateTime(2025, 11, 5),
                CantidadPersonas = 2,
                TipoHabitacionNombre = "Doble"
            };

            // 2) combos en duro
            var vm = new ReservaAsignacionVm
            {
                Reserva = reserva,
                Empresas = new List<SelectListItem>
        {
            new SelectListItem{ Value="", Text="Seleccione..." },
            new SelectListItem{ Value="constructora_abc", Text="Constructora ABC Ltda." },
            new SelectListItem{ Value="minera_xyz", Text="Minera XYZ S.A." },
            new SelectListItem{ Value="servicios_tech", Text="Servicios Tech SpA" },
            new SelectListItem{ Value="transportes_sur", Text="Transportes del Sur" }
        },
                TiposEmpresa = new List<SelectListItem>
        {
            new SelectListItem{ Value="", Text="Seleccione..." },
            new SelectListItem{ Value="contratista", Text="Contratista Principal" },
            new SelectListItem{ Value="subcontratista", Text="Subcontratista" },
            new SelectListItem{ Value="proveedor", Text="Proveedor de Servicios" },
            new SelectListItem{ Value="mandante", Text="Empresa Mandante" }
        },
                Jornadas = new List<SelectListItem>
        {
            new SelectListItem{ Value="", Text="Seleccione..." },
            new SelectListItem{ Value="7x7", Text="7x7" },
            new SelectListItem{ Value="14x7", Text="14x7" },
            new SelectListItem{ Value="20x10", Text="20x10" },
            new SelectListItem{ Value="permanente", Text="Permanente" }
        },
                Horarios = new List<SelectListItem>
        {
            new SelectListItem{ Value="", Text="Seleccione..." },
            new SelectListItem{ Value="diurno", Text="Diurno (08:00 - 20:00)" },
            new SelectListItem{ Value="nocturno", Text="Nocturno (20:00 - 08:00)" },
            new SelectListItem{ Value="mixto", Text="Mixto / Rotativo" },
            new SelectListItem{ Value="administrativo", Text="Administrativo (09:00 - 18:00)" }
        },
                Generos = new List<SelectListItem>
        {
            new SelectListItem{ Value="", Text="Seleccione..." },
            new SelectListItem{ Value="masculino", Text="Masculino" },
            new SelectListItem{ Value="femenino", Text="Femenino" },
            new SelectListItem{ Value="mixto", Text="Mixto" },
        },
                Habitaciones = new List<HabitacionDisponibleDto>
        {
            new HabitacionDisponibleDto{
                Numero = "201", Tipo="Doble", Capacidad=2, PrecioNoche=150,
                Caracteristicas="Vista a la ciudad", Estado="disponible"
            },
            new HabitacionDisponibleDto{
                Numero = "202", Tipo="Doble", Capacidad=2, PrecioNoche=150,
                Caracteristicas="Vista al jardín", Estado="asignada", EmpresaAsignada="Constructora ABC Ltda."
            },
            new HabitacionDisponibleDto{
                Numero = "203", Tipo="Doble", Capacidad=2, PrecioNoche=180,
                Caracteristicas="Vista al mar", Estado="disponible"
            }
        }
            };

            return PartialView("~/Views/Reservas/_AsignarReserva.cshtml", vm);
        }
        [HttpGet]
        public async Task<ActionResult> TipoHabitacionesCombo()
        {
            var token = GetBearer();
            var data = await _api.TiposHabitacionAsync(token) ?? new List<ComboItemDto>();

            var resp = data.Select(d => new
            {
                id = d.Id,
                value = d.Value,
                text = d.Text
            });

            return Json(new { ok = true, data = resp }, JsonRequestBehavior.AllowGet);
        }
    }
}
