using Front_Hoteleria.Dto.Calendario;
using Front_Hoteleria.Services.Calendario;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class CalendarioController : Controller
    {
        private readonly ICalendarioService _api;

        public CalendarioController() : this(new CalendarioService()) { }

        public CalendarioController(ICalendarioService api)
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
                Trace.TraceError("[CalendarioController.GetBearer] " + ex);
                return null;
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View("~/Views/Calendario/Index.cshtml");
        }

        // ===== DASHBOARD =====
        [HttpGet]
        public async Task<ActionResult> Resumen()
        {
            var token = GetBearer();
            var dto = await _api.ResumenAsync(token);
            if (dto == null)
            {
                dto = new CalendarioKpiDto
                {
                    TotalHabitaciones = 20,
                    OcupadasHoy = 89,
                    EnMantenimiento = 12,
                    EnSanitizacion = 8
                };
            }

            return PartialView("~/Views/Calendario/_DashboardCalendario.cshtml", dto);
        }
        // ====== NUEVO: GET /Calendario/Bloquear
        [HttpGet]
        public async Task<ActionResult> Bloquear(string habitacionId = null)
        {
            var token = GetBearer();

            // intentamos traer las habitaciones desde la API
            var habitaciones = await _api.ListarHabitacionesAsync(token);

            // si la API no devolvió nada, dejamos 1..20 en duro
            if (habitaciones == null || habitaciones.Count == 0)
            {
                habitaciones = new List<string>();
                for (int i = 1; i <= 20; i++)
                    habitaciones.Add(i.ToString("D4"));
            }

            ViewBag.Habitaciones = habitaciones;

            var dto = new CalendarioBloqueoDto
            {
                HabitacionId = habitacionId,
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddDays(1)
            };

            return PartialView("~/Views/Calendario/_BloquearHabitacion.cshtml", dto);
        }
        [HttpGet]
        public async Task<ActionResult> ProgramarMantenimiento(string habitacionId = null)
        {
            List<string> habitaciones = null;
            var token = GetBearer();

            try
            {
                habitaciones = await _api.ListarHabitacionesAsync(token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("[CalendarioController.ProgramarMantenimiento] " + ex);
            }

            if (habitaciones == null || habitaciones.Count == 0)
            {
                habitaciones = new List<string>();
                for (int i = 1; i <= 20; i++)
                    habitaciones.Add(i.ToString("D4"));
            }

            ViewBag.Habitaciones = habitaciones;

            var dto = new CalendarioMantenimientoDto
            {
                HabitacionId = habitacionId,
                FechaInicio = DateTime.Today,
                DuracionDias = 1,
                Tipo = "preventive"
            };

            // Asegúrate de que esta ruta exista exactamente así
            return PartialView("~/Views/Calendario/_ProgramarMantenimiento.cshtml", dto);
        }


        // ====== POST: /Calendario/ProgramarMantenimiento
        [HttpPost]
        public async Task<ActionResult> ProgramarMantenimiento(CalendarioMantenimientoDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.HabitacionId))
                return Json(new { ok = false, msg = "Datos incompletos" });

            var token = GetBearer();

            try
            {
                var okApi = await _api.ProgramarMantenimientoAsync(dto, token);
                // si la api no está lista devolvemos ok igual
                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("[CalendarioController.ProgramarMantenimiento] " + ex);
                return Json(new { ok = false, msg = "Error al programar mantenimiento" });
            }
        }

        [HttpGet]
        public async Task<ActionResult> ProgramarSanitizacion(string habitacionId = null)
        {
            var token = GetBearer();
            List<string> habitaciones = null;

            try
            {
                habitaciones = await _api.ListarHabitacionesAsync(token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("[CalendarioController.ProgramarSanitizacion] " + ex);
            }

            if (habitaciones == null || habitaciones.Count == 0)
            {
                habitaciones = new List<string>();
                for (int i = 1; i <= 20; i++)
                    habitaciones.Add(i.ToString("D4"));
            }

            ViewBag.Habitaciones = habitaciones;

            var dto = new CalendarioSanitizacionDto
            {
                HabitacionId = habitacionId,
                FechaInicio = DateTime.Today,
                DuracionHoras = 4,
                Tipo = "routine"
            };

            return PartialView("~/Views/Calendario/_ProgramarSanitizacion.cshtml", dto);
        }

        [HttpPost]
        public async Task<ActionResult> ProgramarSanitizacion(CalendarioSanitizacionDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.HabitacionId))
                return Json(new { ok = false, msg = "Datos incompletos" });

            var token = GetBearer();

            try
            {
                // si tu API aún no tiene este endpoint, simula OK
                var okApi = await _api.ProgramarSanitizacionAsync(dto, token);
                if (!okApi)
                {
                    // maqueta: igual devolvemos ok
                }
                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("[CalendarioController.ProgramarSanitizacion] " + ex);
                return Json(new { ok = false, msg = "Error al programar la sanitización" });
            }
        }

        // ====== NUEVO: POST /Calendario/Bloquear
        [HttpPost]
        public async Task<ActionResult> Bloquear(CalendarioBloqueoDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.HabitacionId))
                return Json(new { ok = false, msg = "Datos incompletos" });

            var token = GetBearer();

            try
            {
                // llamamos a la API real si la tienes
                var okApi = await _api.BloquearHabitacionAsync(dto, token);

                // si la api no está lista, igual devolvemos OK para la maqueta
                if (!okApi)
                {
                    // aquí puedes loguear
                }

                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("[CalendarioController.Bloquear] " + ex);
                return Json(new { ok = false, msg = "Error al bloquear la habitación" });
            }
        }
        // ===== TABLA (calendario) =====
        [HttpGet]
        public async Task<ActionResult> Tabla(string habitacion, string estado)
        {
            var token = GetBearer();

            var lista = await _api.ListarAsync(habitacion, estado, token);
            if (lista == null || !lista.Any())
            {
                // si la API no respondió, igual devolvemos algunos en duro
                lista = new List<CalendarioEventoDto>
                {
                    new CalendarioEventoDto {
                        Id = "CAL-001", HabitacionId = "0001", HabitacionNombre = "0001",
                        Titulo = "Ocupada", FechaInicio = DateTime.Today.AddDays(1),
                        FechaFin = DateTime.Today.AddDays(3), Tipo = "occupied", Color = "#d9534f"
                    },
                    new CalendarioEventoDto {
                        Id = "CAL-002", HabitacionId = "0002", HabitacionNombre = "0002",
                        Titulo = "Mantenimiento", FechaInicio = DateTime.Today.AddDays(2),
                        FechaFin = DateTime.Today.AddDays(2), Tipo = "maintenance", Color = "#f0ad4e"
                    }
                };
            }

            return PartialView("~/Views/Calendario/_TablaCalendario.cshtml", lista);
        }

        // ===== FORMULARIO (modal) =====
        [HttpGet]
        public async Task<ActionResult> Upsert(string id, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var token = GetBearer();
            CalendarioEventoDto dto = null;

            if (!string.IsNullOrWhiteSpace(id))
            {
                dto = await _api.ObtenerPorIdAsync(id, token);
            }

            if (dto == null)
            {
                dto = new CalendarioEventoDto
                {
                    Id = id,
                    Titulo = "",
                    FechaInicio = fechaInicio ?? DateTime.Today,
                    FechaFin = fechaFin ?? DateTime.Today,
                    Tipo = "maintenance"
                };
            }

            // para el combo de habitaciones: en la maqueta eran 20
            ViewBag.Habitaciones = Enumerable.Range(1, 20)
                .Select(i => i.ToString("D4"))
                .ToList();

            return PartialView("~/Views/Calendario/_UpsertCalendario.cshtml", dto);
        }

        [HttpPost]
        public async Task<ActionResult> Guardar(CalendarioEventoDto dto)
        {
            var token = GetBearer();
            if (dto == null)
                return Json(new { ok = false, msg = "Datos vacíos" });

            try
            {
                bool ok;
                if (string.IsNullOrWhiteSpace(dto.Id))
                    ok = await _api.CrearAsync(dto, token);
                else
                    ok = await _api.ActualizarAsync(dto, token);

                // si la API falló, devolvemos ok=true para la maqueta
                if (!ok)
                    ok = true;

                return Json(new { ok = ok });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CalendarioController.Guardar] " + ex);
                return Json(new { ok = false, msg = "Error al guardar" });
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
                Trace.TraceError("[CalendarioController.Eliminar] " + ex);
                return Json(new { ok = false, msg = "Error al eliminar" });
            }
        }
    }
}
