using Front_Hoteleria.Dto.Reserva;
using Front_Hoteleria.Services.ReclamosHuesped; // así lo tienes tú
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class ReclamosHuespedController : Controller
    {
        // OJO: el nombre del servicio lo dejas como lo tienes en tu proyecto
        private readonly IReclamosHuespedService _api;

        // ctor por defecto
        public ReclamosHuespedController() : this(new ReclamosHuespedService())
        {
        }

        // ctor inyectable
        public ReclamosHuespedController(IReclamosHuespedService api)
        {
            _api = api;
        }

        // =========================
        // TOKEN
        // =========================
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
                Trace.TraceError($"[GetBearer] Error leyendo token: {ex}");
                return null;
            }
        }

        // =========================
        // INDEX (elige vista según perfil)
        // =========================
        [HttpGet]
        public ActionResult Index()
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            var perfilObj = Session["IdPerfil"];
            if (perfilObj == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            // asumo que lo guardas como int
            var perfil = Convert.ToInt32(perfilObj);

            switch (perfil)
            {
                case 1: // admin
                    return View("~/Views/Reservas/Index.cshtml");
                case 2: // huésped
                    return View("~/Views/Huesped/Reclamos/Index.cshtml");
                default:
                    return RedirectToAction("Login", "Account");
            }
        }

        // GET: /ReservasHuesped
       

        // POST: /ReservasHuesped/Tabla
        // la vista Index hace un POST con filtros y espera HTML
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Tabla(string Codigo, string Estado, DateTime? Desde, DateTime? Hasta)
        {
            // acá iría tu llamada a la API para traer las reservas del huésped
            // por ahora solo devolvemos el parcial con la tabla
            return PartialView("~/Views/Huesped/Reservas/_TablaMisReservas.cshtml");
        }

        // GET: /ReservasHuesped/Nueva
        // se carga dentro del modal
        [HttpGet]
        public ActionResult Nueva()
        {
            // si tu _UpsertReserva.cshtml necesita combos, los llenas con ViewBag acá
            // ViewBag.Habitaciones = ...
            // ViewBag.TiposReserva = ...

            return PartialView("~/Views/Huesped/Reservas/_UpsertReserva.cshtml");
        }

        // OPCIONAL: si el formulario del modal hace POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(FormCollection form)
        {
            // acá tomas los campos del form y los mandas a la API
            // por ahora devolvemos OK para que el JS cierre el modal
            return Json(new { ok = true });
        }
        [HttpGet]
        public ActionResult TablaReclamos()
        {
            // por ahora no traemos datos del backend,
            // solo devolvemos la estructura de la tabla
            return PartialView("~/Views/Huesped/Reclamos/_TablaReclamos.cshtml");
        }

        // =========================
        // RECLAMOS (HUÉSPED)
        // =========================
        // esta es la página que me dijiste "pertenece a huesped"
        [HttpGet]
        public ActionResult Reclamos()
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            // vista que te dejé antes: /Views/HuespedReclamos/Index.cshtml
            return View("~/Views/HuespedReclamos/Index.cshtml");
        }

        // =========================
        // TABLA PARCIAL DE RESERVAS (HUÉSPED)
        // =========================
        [HttpGet]
        public async Task<ActionResult> TablaPartial(
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int? idEstadoReserva,
            int? idtiporeserva)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult(401, "Sesión expirada");

                var filtro = new ReservaTrabajadorDto
                {
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta,
                    IdEstadoReserva = idEstadoReserva ?? 0,
                    IdTipoReserva = idtiporeserva ?? 0
                };

                // este método está en tu service
                var data = await _api.ReservasDisponiblesTrabajadorAsync(filtro, token);

                return PartialView("~/Views/Reservas/_TablaReserva.cshtml", data);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[TablaPartial] {ex}");
                return new HttpStatusCodeResult(500, "Error al cargar reservas");
            }
        }

        // =========================
        // FORMULARIO DE RESERVA (HUÉSPED)
        // =========================
        [HttpGet]
        public async Task<ActionResult> Upsert()
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult(401);

            // combos de ejemplo; tú los puedes cargar desde la API
            ViewBag.Habitaciones = new SelectList(new[]
            {
                new { Id = 1, Nombre = "Individual" },
                new { Id = 2, Nombre = "Grupal" },
                new { Id = 3, Nombre = "Corporativa" }
            }, "Id", "Nombre");

            ViewBag.TiposReserva = new SelectList(new[]
            {
                new { Id = 1, Nombre = "Individual" },
                new { Id = 2, Nombre = "Grupal" },
                new { Id = 3, Nombre = "Corporativa" }
            }, "Id", "Nombre");

            ViewBag.IdTrabajador = Session["IdTrabajador"];

            var dto = new ReservaTrabajadorDto
            {
                FechaDesde = DateTime.Today,
                FechaHasta = DateTime.Today.AddDays(1),
                IdEstadoReserva = 1
            };

            return PartialView("~/Views/Reservas/_UpsertReserva.cshtml", dto);
        }

        // =========================
        // CREAR RESERVA (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearReserva(ReservaTrabajadorDto dto)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada");

                if (dto == null)
                    return new HttpStatusCodeResult(400, "Datos inválidos");

                if (dto.IdHabitacion <= 0 || !dto.FechaDesde.HasValue || !dto.FechaHasta.HasValue)
                    return new HttpStatusCodeResult(400, "Campos obligatorios faltantes");

                if (dto.IdEstadoReserva == 0)
                    dto.IdEstadoReserva = 1; // ingresada

                var ok = await _api.CrearReservaTrabajadorAsync(dto, token);
                if (!ok)
                    return new HttpStatusCodeResult(500, "No se pudo crear la reserva.");

                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[CrearReserva] {ex}");
                return new HttpStatusCodeResult(500, "Error al crear la reserva.");
            }
        }

        // =========================
        // DASHBOARD PARCIAL
        // =========================
        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada o sin autenticación.");

                var dto = await _api.DashboardReservasAsync(token)
                          ?? new ReservaDashboardDto();

                return PartialView("~/Views/Reservas/_DashboardReserva.cshtml", dto);
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[Dashboard] Error HTTP: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.BadGateway, "No se pudo comunicar con la API de dashboard.");
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[Dashboard] Timeout: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.GatewayTimeout, "La consulta de dashboard excedió el tiempo de espera.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Dashboard] Error: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al cargar el dashboard.");
            }
        }

        // =========================
        // ELIMINAR (AJAX)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Eliminar(int idReserva)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized);

                // en tu versión tenías "idHabitacion", pero lo lógico es eliminar por idReserva
                var ok = await _api.EliminarReservaAsync(idReserva, token);
                if (!ok)
                    return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "No se pudo eliminar.");

                return new HttpStatusCodeResult((int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Eliminar] Error: {ex}");
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al eliminar la reserva.");
            }
        }
    }
}
