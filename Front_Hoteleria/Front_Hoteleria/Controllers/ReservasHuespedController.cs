using Front_Hoteleria.Dto.Huesped;

using Front_Hoteleria.Services.ReservasHuesped;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class ReservasHuespedController : Controller
    {
        private readonly IReservaHuespedService _api;

        public ReservasHuespedController() : this(new ReservaHuespedService()) { }
        public ReservasHuespedController(IReservaHuespedService api) { _api = api; }

        // -------------------------------
        //  TOKEN
        // -------------------------------
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

        // -------------------------------
        //  INDEX
        // -------------------------------
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            var perfil = Session["IdPerfil"];
            if (perfil == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            switch (Convert.ToInt32(perfil))
            {
                case 1: // admin
                    return View("~/Views/Reservas/Index.cshtml");

                case 2: // huésped
                        // Filtro "todas mis reservas"
                    var filtro = new ReservaHuespedDto
                    {
                        // si tu API filtra por trabajador, puedes setearlo aquí
                        // IdTrabajador = (int?)Session["IdTrabajador"]
                    };

                    var lista = await _api.ListarReservasHuespedAsync(filtro, token)
                                ?? new List<ReservaHuespedDto>();

                    // KPIs básicos
                    ViewBag.TotalReservas = lista.Count;
                    ViewBag.Pendientes = lista.Count(r => r.IdEstadoReserva == 1); // Pendiente
                    ViewBag.Aprobadas = lista.Count(r => r.IdEstadoReserva == 2); // Aprobada

                    // Próximas reservas (3 más cercanas a hoy)
                    ViewBag.Proximas = lista
                        .Where(r => r.FechaDesde >= DateTime.Today)
                        .OrderBy(r => r.FechaDesde)
                        .Take(3)
                        .ToList();

                    return View("~/Views/Huesped/Reservas/Index.cshtml");

                default:
                    return RedirectToAction("Login", "Account");
            }
        }

        // -------------------------------
        //  TABLA (PARCIAL)
        // -------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Tabla(string Codigo, int? Estado, DateTime? Desde, DateTime? Hasta)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada");

                var filtro = new ReservaHuespedDto
                {
                    FiltroCodigo = Codigo,
                    FiltroIdEstado = Estado,
                    FiltroDesde = Desde,
                    FiltroHasta = Hasta
                };

                var lista = await _api.ListarReservasHuespedAsync(filtro, token)
                            ?? new List<ReservaHuespedDto>();

                return PartialView("~/Views/Huesped/Reservas/_TablaReservasHuesped.cshtml", lista);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasHuesped.Tabla] " + ex);
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al cargar las reservas.");
            }
        }

        // -------------------------------
        //  NUEVA RESERVA (GET)
        // -------------------------------
        [HttpGet]
        public ActionResult Nueva()
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized);

            var dto = new ReservaHuespedDto
            {
                FechaDesde = DateTime.Today,
                FechaHasta = DateTime.Today.AddDays(1),
                FechaSolicitud = DateTime.Today,
                DiasEstadia = 1,
                IdEstadoReserva = 1 // Pendiente
            };

            return PartialView("~/Views/Huesped/Reservas/_UpsertReservaHuesped.cshtml", dto);
        }

        // -------------------------------
        //  EDITAR (GET)
        // -------------------------------
        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized);

            var dto = await _api.ObtenerReservaHuespedPorIdAsync(id, token);
            if (dto == null)
                return new HttpStatusCodeResult((int)HttpStatusCode.NotFound);

            return PartialView("~/Views/Huesped/Reservas/_EditReservaHuesped.cshtml", dto);
        }
        // -------------------------------
        //  ENCUESTA (GET)
        // -------------------------------
        [HttpGet]
        public ActionResult Encuesta(int tipo = 1, int? idReserva = null)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized);

            var dto = new EncuestaSatisfaccionDto
            {
                TipoEncuesta = tipo,
                IdReserva = idReserva
            };

            return PartialView("~/Views/Huesped/Reservas/_EncuestaSatisfaccion.cshtml", dto);
        }

        // -------------------------------
        //  GUARDAR ENCUESTA (POST)
        // -------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GuardarEncuesta(EncuestaSatisfaccionDto dto)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { ok = false, message = "Sesión expirada." });

                var ok = await _api.RegistrarEncuestaAsync(dto, token);

                return Json(new
                {
                    ok,
                    message = ok ? "Encuesta enviada. ¡Gracias por su opinión!" :
                                   "No se pudo guardar la encuesta."
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasHuesped.GuardarEncuesta] " + ex);
                return Json(new { ok = false, message = "Error al guardar la encuesta." });
            }
        }
            // -------------------------------
            //  DETALLE (GET)
            // -------------------------------
            [HttpGet]
        public async Task<ActionResult> Detalle(int id)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized);

            var dto = await _api.ObtenerReservaHuespedPorIdAsync(id, token);
            if (dto == null)
                return new HttpStatusCodeResult((int)HttpStatusCode.NotFound);

            return PartialView("~/Views/Huesped/Reservas/_DetalleReservaHuesped.cshtml", dto);
        }

        // -------------------------------
        //  GUARDAR (CREATE / UPDATE)
        // -------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Guardar(ReservaHuespedDto dto)
        {
            try
            {

                dto.IdTrabajador = 1; // agregar el rut
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { ok = false, message = "Sesión expirada." });

                if (dto == null ||
                    //string.IsNullOrWhiteSpace(dto.Nombre) ||
                    //string.IsNullOrWhiteSpace(dto.Apellido) ||
                    string.IsNullOrWhiteSpace(dto.Email) ||
                    dto.FechaDesde == default ||
                    dto.FechaHasta == default)
                {
                    return Json(new { ok = false, message = "Complete todos los campos obligatorios." });
                }



                dto.DiasEstadia = (int)(dto.FechaHasta.Date - dto.FechaDesde.Date).TotalDays;

                bool ok;

                if (dto.IdReserva == 0)
                {
                    dto.IdEstadoReserva = 1; // Pendiente
                    ok = await _api.CrearReservaHuespedAsync(dto, token);
                }
                else
                {
                    ok = await _api.ActualizarReservaHuespedAsync(dto, token);
                }

                return Json(new
                {
                    ok,
                    message = ok ? "Reserva guardada correctamente." : "No se pudo guardar la reserva."
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasHuesped.Guardar] " + ex);
                return Json(new { ok = false, message = "Error al guardar la reserva." });
            }
        }

        // -------------------------------
        //  ELIMINAR
        // -------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { ok = false, message = "Sesión expirada." });

                var ok = await _api.EliminarReservaHuespedAsync(id, token);

                return Json(new
                {
                    ok,
                    message = ok ? "Reserva eliminada correctamente." : "No se pudo eliminar la reserva."
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasHuesped.Eliminar] " + ex);
                return Json(new { ok = false, message = "Error al eliminar la reserva." });
            }
        }
    }
}
