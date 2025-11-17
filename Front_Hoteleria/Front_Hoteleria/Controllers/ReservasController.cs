using Font_Hoteleria.Dto.Trabajadores;
using Front_Hoteleria.Dto;
using Front_Hoteleria.Dto.Empresa;
using Front_Hoteleria.Dto.Habitacion;
using Front_Hoteleria.Dto.Inventario;
using Front_Hoteleria.Dto.Reserva;
using Front_Hoteleria.Services.Empresa;
using Front_Hoteleria.Services.Habitacion;
using Front_Hoteleria.Services.Reservas;
using Front_Hoteleria.Services.Trabajadores;
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
    public class ReservasController : Controller
    {
        private readonly IReservaService _api;
        private readonly ITrabajadoresService _apitraservice;
        private readonly IHabitacionService _apihabservice;
        private readonly IEmpresaService _apiempservice;

        public ReservasController() : this(new ReservaService(), new TrabajadoresService(), new HabitacionService(), new EmpresaService()) { }

        public ReservasController(IReservaService api, ITrabajadoresService apitraservice, IHabitacionService apihabservice, IEmpresaService apiempservice)
        {
            _api = api;
            _apitraservice= apitraservice;
            _apihabservice = apihabservice;
            _apiempservice = apiempservice;
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
                //dto = new ReservaKPIDto
                //{
                //    Pendientes = 5,
                //    Confirmadas = 18,
                //    Rechazadas = 2,
                //    Total = 25
                //};
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
            var strEstado = Request["estado"];   // viene "1","2","3","4","5"
            

            
            if (!string.IsNullOrWhiteSpace(strDesde) &&
                DateTime.TryParse(strDesde, out var d1))
                fechaDesde = d1;

            if (!string.IsNullOrWhiteSpace(strHasta) &&
                DateTime.TryParse(strHasta, out var d2))
                fechaHasta = d2;

            int? idEstadoReserva = 0;
            if (strEstado == "5") { idEstadoReserva = 0; }
            if (int.TryParse(strEstado, out var idTmp))
            {
                // 1..4 = filtrar por ese estado
                // 5    = "Todos" -> no se filtra (queda null)
                if (idTmp >= 1 && idTmp <= 4)
                    idEstadoReserva = idTmp;
            }

            try
            {
                var token = GetBearer();
                var lista = await _api.ListarAsync(
                    estado: idEstadoReserva,   // <-- cambia el tipo del parámetro en el service a int?
                    //habitacion: habitacion,
                    fechaDesde: fechaDesde,
                    fechaHasta: fechaHasta,
                    bearer: token
                );

                return PartialView("~/Views/Reservas/_TablaReserva.cshtml", lista);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasController.TablaPartial] " + ex);
                return new HttpStatusCodeResult(500, "No se pudo cargar el listado de reservas");
            }
        }
        [HttpPost]
        public async Task<ActionResult> Guardar(ReservaDto dto)
        {
            if (dto == null)
                return Json(new { ok = false, msg = "Datos vacíos" });

            var token = GetBearer();

            try
            {
                bool ok;
                if (dto.IdReserva > 0)
                    ok = await _api.ActualizarAsync(dto, token);
                else
                    ok = await _api.CrearAsync(dto, token);

                if (!ok)
                    return Json(new { ok = false, msg = "No se pudo guardar en la API" });

                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InventarioController.Guardar] " + ex);
                return Json(new { ok = false, msg = "Error inesperado al guardar" });
            }
        }
        // GET: /Reservas/Rechazar?id=RES-006
     [HttpGet]



    public async Task<ActionResult> Rechazar(int idReserva)
    {
        var token = GetBearer();

        // 1) Traer la reserva real desde la API
        var dto = await _api.ObtenerPorIdAsync(idReserva, token);

        // 2) Si no existe, 404
        if (dto == null)
            return HttpNotFound("Reserva no encontrada");

        // 3) Si ya está en estado 3 (rechazada), no permitir rechazar de nuevo
        if (dto.IdEstadoReserva == 3) // 3 = Rechazada
        {
            return new HttpStatusCodeResult(
                (int)HttpStatusCode.Conflict,
                "La reserva ya se encuentra rechazada y no puede volver a rechazarse."
            );
        }

        // 4) Si todo ok, mostrar el modal de rechazo
        return PartialView("~/Views/Reservas/_RechazarReserva.cshtml", dto);
    }


    // POST: /Reservas/Rechazar
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Rechazar(ReservaDto dto)
        {
            var token = GetBearer();

            var dtoOriginal = new ReservaDto();
            dtoOriginal = await _api.ObtenerPorIdAsync(dto.IdReserva, token);
            dtoOriginal.IdMotivoRechazo = dto.IdMotivoRechazo;
            dtoOriginal.ObservacionesRechazo = dto.ObservacionesRechazo;
            if (dto == null || dto.IdReserva <= 0)
                return Json(new { ok = false, msg = "Id de reserva requerido." });

            // Validar motivo y observaciones si quieres obligarlos
            if (dtoOriginal.IdMotivoRechazo == null || dtoOriginal.IdMotivoRechazo <= 0)
                return Json(new { ok = false, msg = "Debe seleccionar un motivo de rechazo." });

            var exito = await _api.EliminarAsync(dtoOriginal, token);

            if (!exito)
                return Json(new { ok = false, msg = "No fue posible rechazar la reserva, intente nuevamente." });

            return Json(new { ok = true, msg = "Reserva rechazada correctamente." });
        }

        // =========================================================
        // GET: /Reservas/Upsert
        // abre el modal de crear/editar
        // =========================================================

        [HttpGet]
  
        public async Task<ActionResult> Upsert(int idReserva = 0)
        {
            var token = GetBearer();
            ReservaDto dto = null;

            if (idReserva > 0)
            {
                dto = await _api.ObtenerPorIdAsync(idReserva, token);
            }

            // nuevo o no encontrado => dto vacío
            if (dto == null)
                dto = new ReservaDto();

            // ================== TIPOS DE HABITACIÓN ==================
            var tipos = await _api.TiposHabitacionAsync(token) ?? new List<ComboItemDto>();

            ViewBag.TiposHabitacion = tipos
                .Select(t =>
                {
                    int idTipo;
                    int.TryParse(t.Id, out idTipo);   // t.Id es string

                    return new SelectListItem
                    {
                        Value = t.Id,                 // se envía como string al select
                        Text = t.Text,
                        Selected = (dto.IdReservaTipoHabitacion == idTipo)
                    };
                })
                .ToList();

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
                // Recargar combos
                var tiposInvalid = await _api.TiposHabitacionAsync(GetBearer()) ?? new List<ComboItemDto>();
                ViewBag.TiposHabitacion = tiposInvalid
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id,
                        Text = t.Text
                    }).ToList();

                return PartialView("~/Views/Reservas/_UpsertReserva.cshtml", dto);
            }

            var token = GetBearer();

            dto.IdHabitacion = 2;
            dto.FechaCheckIN = dto.FechaDesde;
            dto.FechaCheckOut = dto.FechaHasta;
            dto.IdEstadoReserva = 1;

            // Buscar trabajador por RUT
            var rut = dto.RutHuesped.Replace(".","");
            var listaTrabajadores = await _apitraservice.BuscarTrabajadorAsync(rut, token);
            var dtotrabajadores = listaTrabajadores.FirstOrDefault();

            // 🚨 RUT no existe: devolvemos JSON para que el front haga alert
            if (dtotrabajadores == null)
            {
                return Json(new
                {
                    ok = false,
                    message = "El RUT ingresado no existe como trabajador en el sistema."
                });
            }

            try
            {
                bool ok;
                if (dto.IdReserva == 0)
                    ok = await _api.CrearAsync(dto, token);
                else
                    ok = await _api.ActualizarAsync(dto, token);

                if (ok)
                    return Json(new { ok = true });

                ModelState.AddModelError("", "No se pudo guardar la reserva en la API.");

                var tipos = await _api.TiposHabitacionAsync(token) ?? new List<ComboItemDto>();
                ViewBag.TiposHabitacion = tipos
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id,
                        Text = t.Text
                    }).ToList();

                return PartialView("~/Views/Reservas/_UpsertReserva.cshtml", dto);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasController.Upsert POST] " + ex);
                ModelState.AddModelError("", "Error inesperado al guardar la reserva.");

                var tipos = await _api.TiposHabitacionAsync(token) ?? new List<ComboItemDto>();
                ViewBag.TiposHabitacion = tipos
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id,
                        Text = t.Text
                    }).ToList();

                return PartialView("~/Views/Reservas/_UpsertReserva.cshtml", dto);
            }
        }


        // =========================================================
        // POST: /Reservas/Eliminar
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Eliminar(ReservaDto dto)
        {
            if (dto.IdReserva==0)
                return Json(new { ok = false, msg = "Id requerido" });

            var token = GetBearer();
            try
            {
                var ok = await _api.EliminarAsync(dto, token);
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
    public async Task<ActionResult> Asignar(int idReserva)
    {
        var token = GetBearer();
        ReservaDto dto = null;

        // 1) Obtener reserva
        if (idReserva > 0)
            dto = await _api.ObtenerPorIdAsync(idReserva, token);

        if (dto == null)
            dto = new ReservaDto { IdReserva = idReserva };

        // 2) EMPRESAS -> ViewBag
        var empresasDto = await _apiempservice.ListarComboAsync(true, null, token)
                         ?? new List<EmpresaDto>();

        var empresasSelect = empresasDto
            .Select(e => new SelectListItem
            {
                Value = e.IdEmpresa.ToString(),          // lo que se postea
                Text = e.Nombre                         // lo que se muestra
                                                        // si quieres incluir RUT:
                                                        // Text = $"{e.Nombre} ({e.Rut})"
            })
            .ToList();

        // Opción "Seleccione..." al inicio
        empresasSelect.Insert(0, new SelectListItem
        {
            Value = "",
            Text = "Seleccione..."
        });

        ViewBag.Empresas = empresasSelect;

            // 3) HABITACIONES desde servicio
            var habitacionesSource = await _apihabservice.HabitacionesDisponiblesAsync(1, token)
                             ?? new List<HabitacionDto>();

            var habitacionesVm = habitacionesSource
                .Select(h =>
                {
                    var capNombre = h.Capacidad == 1
                        ? "Individual"
                        : (h.Capacidad == 2 ? "Doble" : "Familiar");

                    return new HabitacionDisponibleDto
                    {
                        Numero = h.NombreHabitacion,              // o h.IdHabitacion.ToString("000")
                        Tipo = h.IdTipoHabitacion.ToString(),
                        Capacidad = h.Capacidad,
                        CapacidadNombre = capNombre,
                        PrecioNoche = h.Precio,
                        Caracteristicas = h.Motivo,
                        Estado = "disponible",
                        EmpresaAsignada = null
                    };
                })
                .ToList();


            // 4) ViewModel (Empresas ya NO se usa aquí)
            var vm = new ReservaAsignacionVm
        {
            Reserva = dto,

            Empresas = new List<SelectListItem>(), // no se usa en la vista, puedes dejarla vacía

            TiposEmpresa = new List<SelectListItem>
        {
            new SelectListItem{ Value = "",  Text = "Seleccione..." },
            new SelectListItem{ Value = "1", Text = "Contratista Principal" },
            new SelectListItem{ Value = "2", Text = "Subcontratista" },
            new SelectListItem{ Value = "3", Text = "Proveedor de Servicios" },
            new SelectListItem{ Value = "4", Text = "Empresa Mandante" }
        },
            Jornadas = new List<SelectListItem>
        {
            new SelectListItem{ Value = "",  Text = "Seleccione..." },
            new SelectListItem{ Value = "1", Text = "7x7" },
            new SelectListItem{ Value = "2", Text = "14x7" },
            new SelectListItem{ Value = "3", Text = "20x10" },
            new SelectListItem{ Value = "4", Text = "Permanente" }
        },
            Horarios = new List<SelectListItem>
        {
            new SelectListItem{ Value = "",  Text = "Seleccione..." },
            new SelectListItem{ Value = "1", Text = "Diurno (08:00 - 20:00)" },
            new SelectListItem{ Value = "2", Text = "Nocturno (20:00 - 08:00)" },
            new SelectListItem{ Value = "3", Text = "Mixto / Rotativo" },
            new SelectListItem{ Value = "4", Text = "Administrativo (09:00 - 18:00)" }
        },
            Generos = new List<SelectListItem>
        {
            new SelectListItem{ Value = "",  Text = "Seleccione..." },
            new SelectListItem{ Value = "1", Text = "Masculino" },
            new SelectListItem{ Value = "2", Text = "Femenino" },
            new SelectListItem{ Value = "3", Text = "Mixto" }
        },

            Habitaciones = habitacionesVm
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
