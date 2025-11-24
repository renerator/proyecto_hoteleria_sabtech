using Front_Hoteleria.Dto.Empresa;      // ajusta al namespace real
using Front_Hoteleria.Dto.Habitacion;   // ajusta al namespace real
using Front_Hoteleria.Dto.SolicitudServicio;
using Front_Hoteleria.Dto.Servicio;
using Front_Hoteleria.Services.Empresa;
using Front_Hoteleria.Services.Habitacion;
using Front_Hoteleria.Services.SolicitudServicio;
using Front_Hoteleria.Services.Trabajadores;
using Front_Hoteleria.Services.Servicio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;

namespace Front_Hoteleria.Controllers
{
    public class SolicitudServicioController : Controller
    {
        private readonly ISolicitudServicioService _api;
        private readonly IEmpresaService _empService;
        private readonly IHabitacionService _habService;
        private readonly ITrabajadoresService _trabService;
        private readonly IServicioService _serService;

        public SolicitudServicioController()
            : this(new SolicitudServicioService(),
                   new EmpresaService(),
                   new HabitacionService(), new TrabajadoresService(), new ServicioService())
        {
        }

        public SolicitudServicioController(
            ISolicitudServicioService api,
            IEmpresaService empService,
            IHabitacionService habService, ITrabajadoresService trabService, IServicioService serService)
        {
            _api = api;
            _empService = empService;
            _habService = habService;
            _trabService = trabService;
            _serService=serService;
        }

        // =============== TOKEN ===============
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

        // =============== INDEX ===============
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Solicitudes de Servicios";
            return View("~/Views/SolicitudServicio/Index.cshtml");
        }

        // =============== DASHBOARD ===============
        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

            var kpi = await _api.ObtenerKpiAsync(token);
            return PartialView("~/Views/SolicitudServicio/_DashboardServicio.cshtml", kpi);
        }

        // =============== PANELES ===============
        [HttpGet]
        public ActionResult Paneles()
        {
            return PartialView("~/Views/SolicitudServicio/_PanelesServicio.cshtml");
        }

        // =============== TABLA ===============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Tabla(int idEstado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

            var lista = await _api.ListarSolicitudesVigentesAsync(fechaInicio, fechaFin, idEstado, token)
                        ?? new List<SolicitudServicioDto>();

            return PartialView("~/Views/SolicitudServicio/_TablaServicio.cshtml", lista);
        }

        // =============== DETALLE ===============
        [HttpGet]
        public async Task<ActionResult> Detalle(int idSolicitud)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

            if (idSolicitud <= 0)
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "Id inválido.");

            var dto = await _api.ObtenerSolicitudAsync(idSolicitud, token);
            if (dto == null)
                return new HttpStatusCodeResult((int)HttpStatusCode.NotFound, "Solicitud no encontrada.");

            return PartialView("~/Views/SolicitudServicio/_DetalleServicio.cshtml", dto);
        }

        // =============== UPSERT (NUEVA / EDITAR) ===============
        [HttpGet]
        public async Task<ActionResult> Upsert(int? id)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

            SolicitudServicioDto model;

            if (id.HasValue && id.Value > 0)
            {
                model = await _api.ObtenerSolicitudAsync(id.Value, token)
                        ?? new SolicitudServicioDto { IdSolicitud = 0 };
            }
            else
            {
                model = new SolicitudServicioDto
                {
                    IdSolicitud = 0,
                    FechaSolicitud = DateTime.Now,
                    IdEstadoSolicitud = 1 // Pendiente
                };
            }

            await CargarCombosAsync(model, token);

            return PartialView("~/Views/SolicitudServicio/_UpsertServicio.cshtml", model);
        }

        private async Task CargarCombosAsync(SolicitudServicioDto model, string token)
        {


            // ---------- Servicios ----------
            var servicios = await _serService.ListarServiciosAsync(1, token);

            var listServicios = new List<SelectListItem>();
            foreach (var e in servicios)
            {
                listServicios.Add(new SelectListItem
                {
                    Value = e.IdServicio.ToString(),
                    Text = e.NombreServicio  // ajusta al nombre real
                });
            }
            ViewBag.Servicios = listServicios;
            // ---------- EMPRESAS ----------
            var empresas = await _empService.ListarComboAsync(true, null, token)
                           ?? new List<EmpresaDto>();

            var listEmpresas = new List<SelectListItem>();
            foreach (var e in empresas)
            {
                listEmpresas.Add(new SelectListItem
                {
                    Value = e.IdEmpresa.ToString(),
                    Text = e.Nombre  // ajusta al nombre real
                });
            }
            ViewBag.Empresas = listEmpresas;

            // ---------- HABITACIONES ----------
            var habitaciones = await _habService.HabitacionesDisponiblesAsync(1,token)
                               ?? new List<HabitacionDto>();

            var listHabitaciones = new List<SelectListItem>();
            foreach (var h in habitaciones)
            {
                var texto = string.IsNullOrWhiteSpace(h.NombreHabitacion)
                    ? h.IdHabitacion.ToString()
                    : h.NombreHabitacion;   // o $"{h.Codigo} - {h.NombreHabitacion}"

                listHabitaciones.Add(new SelectListItem
                {
                    Value = h.IdHabitacion.ToString(),
                    Text = texto
                });
            }
            ViewBag.Habitaciones = listHabitaciones;
        }
        // ===================== ASIGNAR PERSONAL =====================
        [HttpGet]
        public async Task<ActionResult> AsignarPersonal(int idSolicitud)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

            if (idSolicitud <= 0)
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "Id inválido.");

            var dto = await _api.ObtenerSolicitudAsync(idSolicitud, token);
            if (dto == null)
                return new HttpStatusCodeResult((int)HttpStatusCode.NotFound, "Solicitud no encontrada.");

            // La vista mostrará los datos de la solicitud y un combo de personal
            return PartialView("~/Views/SolicitudServicio/_AsignarPersonal.cshtml", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AsignarPersonal(int idSolicitud, int? idPersonal, bool asignacionAutomatica)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            var ok = await _api.AsignarPersonalAsync(idSolicitud, idPersonal, asignacionAutomatica, token);
            return Json(new { ok, message = ok ? "Personal asignado." : "No se pudo asignar el personal." });
        }
        // =============== CREAR / MODIFICAR / ASIGNAR / ELIMINAR ===============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Crear(SolicitudServicioDto dto)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            
            var Trabajadores = await _trabService.BuscarTrabajadorAsync(dto.RutSolicitante, token);
            var trabajador = Trabajadores.FirstOrDefault();
            if (trabajador == null )
            {
                return Json(new { ok = false, message = "No se encontró el trabajador para el RUT ingresado." });
            }

            dto.IdSolicitante = trabajador.IdUsuario;   // o solo idTrabajador si es int
            dto.idEstado = true;
            dto.IdEstadoSolicitud = 1; // por ejemplo Pendiente
                                       // dto.IdOrdenTrabajo = ... si corresponde
            dto.IdPersonalAsignado = 1;
            dto.IdOrdenTrabajo = 1;

            var ok = await _api.CrearSolicitudAsync(dto, token);
            return Json(new { ok, message = ok ? "Solicitud creada." : "No se pudo crear la solicitud." });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Modificar(SolicitudServicioDto dto)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            var Trabajadores = await _trabService.BuscarTrabajadorAsync(dto.RutSolicitante, token);
            var trabajador = Trabajadores.FirstOrDefault();
            if (trabajador == null)
            {
                return Json(new { ok = false, message = "No se encontró el trabajador para el RUT ingresado." });
            }

            dto.IdSolicitante = trabajador.IdUsuario;   // o solo idTrabajador si es int
            dto.idEstado = true;
            dto.IdEstadoSolicitud = 1; // por ejemplo Pendiente
                                       // dto.IdOrdenTrabajo = ... si corresponde
            dto.IdPersonalAsignado = 1;
            dto.IdOrdenTrabajo = 1;

            var ok = await _api.ModificarSolicitudAsync(dto, token);
            return Json(new { ok, message = ok ? "Solicitud actualizada." : "No se pudo actualizar la solicitud." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Eliminar(int idSolicitud)
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "Sesión expirada." });

            var ok = await _api.EliminarSolicitudAsync(idSolicitud, token);
            return Json(new { ok, message = ok ? "Solicitud eliminada." : "No se pudo eliminar la solicitud." });
        }
    }
}
