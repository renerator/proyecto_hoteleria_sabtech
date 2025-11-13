using DemoBackend.Dto.BitacoraReserva;
using DemoBackend.Dto.EstadoReserva;
using DemoBackend.Dto.Mantenedores;
using DemoBackend.Dto.Reserva;
using DemoBackend.Models.Reserva;
using DemoBackend.Services;
using DemoBackend.Services.Reserva;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoBackend.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class ReservasController : BaseController
    {
        private readonly IReservaService _reservaService;
        private readonly ILogger _logger;

        public ReservasController(IReservaService reservaService, ILogger<ReservasController> logger)
        {
            _logger = logger;
            _reservaService = reservaService;
        }

        /// <summary>
        /// Servicio que retorna el listado de Reservas según estado
        /// </summary>
        /// <returns>lista Reservas</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("ReservasDisponibles")]
        public ActionResult<List<ReservaDto>> ReservasDisponibles(int idEstadoReserva, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            _logger.LogInformation("GetListaReservas : Inicio proceso lista de Reservas");
            try
            {
                var reservas = _reservaService.GetListaReservaEstado(idEstadoReserva, fechaDesde, fechaHasta);
                if (reservas.Count == 0)
                {
                    _logger.LogInformation("GetListaReservas : No se encontraron reservas.");
                    return new NoContentResult();
                }
                else
                {
                    _logger.LogInformation($"GetListaReservas : Se encontraron {reservas.Count} reservas.");
                    return Ok(reservas);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"GetListaReservas : Error --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Servicio para crear una reserva
        /// </summary>
        /// <returns>lista areas</returns>
        /// <response code="200">Insercion exitosa de datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpPost("SolicitaReserva")]
        public ActionResult SolicitaReserva(ReservaDto ReservaDto)
        {
            try
            {
                var grupoOK = false;

                if (string.IsNullOrEmpty(ReservaDto.MotivoReserva))
                {
                    _logger.LogInformation($"PostCreaHabitacion: Vacio, no se graban datos, retorna OK.");
                    return Ok("Status 200: Error de campos vacios");
                }
                else
                {
                    var resu = _reservaService.VerificaReservaPorId(ReservaDto);
                    if (resu.Count > 0)
                    {
                        return Ok("Status 200: No se puede crear el area, ya existe la reserva: " + ReservaDto.IdReserva);
                    }
                    else
                    {
                        grupoOK = _reservaService.CrearReserva(ReservaDto);
                        if (grupoOK)
                        {
                            return Ok(grupoOK + " OK, Datos insertados");
                        }
                        else
                        {
                            return Ok(grupoOK + " Datos no insertados");
                        }

                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"PostCreaReserva: Error --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }


        [HttpGet("MuestraReserva")]
        public ActionResult<ReservaDto> MuestraReserva([FromQuery] int id)
        {
            _logger.LogInformation("GET MuestraReserva: inicio. Id={Id}", id);

            try
            {
                // armado del filtro que espera tu service
                var filtro = new ReservaDto
                {
                    IdReserva = id
                };

                // tu service devuelve una LISTA (igual que el de servicios)
                var items = _reservaService.VerificaReservaPorId(filtro);

                if (items == null || items.Count == 0)
                {
                    _logger.LogInformation("GET MuestraReserva: sin resultados para Id={Id}.", id);
                    return NoContent();
                }

                // devolvemos solo la primera coincidencia
                var reserva = items.First();

                _logger.LogInformation("GET MuestraReserva: 1 registro encontrado para Id={Id}.", id);
                return Ok(reserva);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GET MuestraReserva: error para Id={Id}.", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Servicio para confirmar una reserva
        /// </summary>
        /// <returns>lista areas</returns>
        /// <response code="200">Insercion exitosa de datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpPost("ConfirmarReserva")]
        public ActionResult ConfirmarReserva(ReservaDto reservaDto)
        {
            try
            {
                if (reservaDto.IdReserva == 0)
                {
                    _logger.LogInformation("PostConfirmarReserva: Falta idReserva.");
                    return Ok("Status 200: Error de campos vacíos");
                }

                var existe = _reservaService.VerificaReservaPorId(reservaDto);
                if (existe.Count == 0)
                    return Ok("No se puede confirmar, la reserva no existe.");

                var confirmada = _reservaService.ModificarReserva(reservaDto);
                if (confirmada)
                    return Ok("Reserva confirmada correctamente.");
                else
                    return Ok("No se pudo confirmar la reserva.");
            }
            catch (Exception e)
            {
                _logger.LogError($"PostConfirmarReserva: Error --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Servicio para modificar una reserva
        /// </summary>
        /// <returns>lista reservas</returns>
        /// <response code="200">Modificación exitosa de datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpPut("ModificaReserva")]
        public ActionResult ModificaReserva(ReservaDto reservaDto)
        {
            try
            {
                if (reservaDto.IdReserva == 0)
                {
                    _logger.LogInformation("PutModificaReserva: Falta idReserva.");
                    return Ok("Status 200: Error de campos vacíos");
                }

                var existe = _reservaService.VerificaReservaPorId(reservaDto);
                if (existe.Count == 0)
                    return Ok($"No se puede modificar, no existe la reserva con Id {reservaDto.IdReserva}");

                var modificada = _reservaService.ModificarReserva(reservaDto);
                if (modificada)
                    return Ok("Reserva modificada correctamente.");
                else
                    return Ok("No se pudo modificar la reserva.");
            }
            catch (Exception e)
            {
                _logger.LogError($"PutModificaReserva: Error --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Servicio para eliminar una reserva
        /// </summary>
        /// <param name="idReserva">Id de la Reserva</param>
        /// <returns>true o false</returns>
        /// <response code="200">Insercion exitosa de datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpDelete("EliminaReserva")]
        public ActionResult EliminaReserva(int idReserva)
        {
            try
            {
                if (idReserva == 0)
                {
                    _logger.LogInformation("DelEliminaReserva: idReserva vacío.");
                    return Ok("Status 200: Error de campos vacíos");
                }

                var reservaDto = new ReservaDto() { IdReserva = idReserva };
                var existe = _reservaService.VerificaReservaPorId(reservaDto);
                if (existe.Count == 0)
                    return Ok($"No se puede eliminar, no existe la reserva con Id {idReserva}");

                var eliminada = _reservaService.EliminarReserva(reservaDto);
                if (eliminada)
                    return Ok("Reserva eliminada correctamente.");
                else
                    return Ok("No se pudo eliminar la reserva.");
            }
            catch (Exception e)
            {
                _logger.LogError($"DelEliminaReserva: Error --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }


        /// <summary>
        /// Servicio que devuelve una lista de reservas según los filtros ingresados.
        /// </summary>
        /// <param name="reservaDto">Objeto con los filtros opcionales de búsqueda.</param>
        /// <returns>Lista de reservas que cumplen los criterios.</returns>
        /// <response code="200">OK - Retorna lista de reservas.</response>
        /// <response code="204">Sin resultados.</response>
        /// <response code="400">Solicitud inválida.</response>
        /// <response code="401">No autorizado.</response>
        /// <response code="403">Acceso denegado.</response>
        /// <response code="500">Error interno.</response>
        [HttpGet("BuscarReservas")]
        public ActionResult<List<ReservaDto>> BuscarReservas(
            [FromQuery] int? idReserva,
            [FromQuery] int? idHabitacion,
            [FromQuery] int? idTrabajador,
            [FromQuery] DateTime? FechaDesde,
            [FromQuery] DateTime? FechaHasta,
            [FromQuery] bool? QuiereTransporte,
            [FromQuery] DateTime? FechaCheckIN,
            [FromQuery] DateTime? FechaCheckOut,
            [FromQuery] int? idEstadoReserva,
            [FromQuery] string? MotivoReserva)
        {
            try
            {
                var filtro = new ReservaDto
                {
                    IdReserva = idReserva ?? 0,
                    IdHabitacion = idHabitacion ?? 0,
                    IdTrabajador = idTrabajador ?? 0,
                    IdEstadoReserva = idEstadoReserva ?? 0,
                
                    FechaDesde = FechaDesde ?? null,
                    FechaHasta = FechaHasta ?? null,

                   
                    QuiereTransporte = QuiereTransporte ?? null,
       

                    FechaCheckIN = FechaCheckIN ?? null,
                    FechaCheckOut = FechaCheckOut ?? null,

                    MotivoReserva = string.IsNullOrWhiteSpace(MotivoReserva) ? null : MotivoReserva!.Trim()
                };

                var resultados = _reservaService.BuscaReservas(filtro);

                if (resultados == null || resultados.Count == 0)
                    return NoContent();

                return Ok(resultados);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetBuscarReservas: error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // -------- DASHBOARD --------
        // Devuelve KPIs del dashboard (nuevas, servicios, checkin, checkout, pendientes, confirmadas, rechazadas, realizadas)
        [HttpGet("dashboardReservas")]
        public IActionResult Dashboard([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] int idHabitacion, int idTipoReserva )
        {
            var data = _reservaService.ObtenerDashboard(desde, hasta, idHabitacion, idTipoReserva);

            // Evitar caché para “tiempo real”
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return Ok(data);
        }

        [HttpPost("CrearBitacoraReserva")]
        public ActionResult CrearBitacoraReserva([FromBody] BitacoraReservaDto dto)
        {
            _logger.LogInformation("PostCrearBitacoraReserva: inicio.");
            try
            {
                if (dto == null) return BadRequest("Datos vacíos.");
                if (dto.IdReserva <= 0) return Ok("Status 200: Error de validación (IdReserva requerido).");

                var ok = _reservaService.CrearBitacoraReserva(dto);
                return Ok(ok ? "Bitácora de reserva creada correctamente." : "No se pudo crear la bitácora.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "PostCrearBitacoraReserva: error.");
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Servicio que retorna el listado de Reservas según estado
        /// </summary>Trabajador
        /// <returns>lista Reservas</returns>
        /// <// -------- DASHBOARD --------
        // Devuelve KPIs del dashboard (nuevas, servicios, checkin, checkout, pendientes, confirmadas, rechazadas, realizadas)
        [HttpGet("dashboardReservasPanelPrincipal")]
        public IActionResult DashboardPanelPrincipal([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {

            try
            {
                var data = _reservaService.ObtenerDashboardPanelPrincipal(desde, hasta);

                // Evitar caché para “tiempo real”
                Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
                Response.Headers["Pragma"] = "no-cache";
                Response.Headers["Expires"] = "0";
                _logger.LogInformation($"DashboardPanelPrincipal :  resultado(s).");
                return Ok(data);
            }
            catch (Exception e)
            {

                _logger.LogError(e, "DashboardPanelPrincipal : error.");
                return StatusCode(500, e.Message);
            }
            
        }
        //code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("ReservasTrabajadorDisponibles")]

       
       

    [HttpGet]
    public ActionResult<List<ReservaTrabajadorDto>> ReservasTrabajadorDisponibles(
    [FromQuery] int? idReserva,
    [FromQuery] int? idHabitacion,
    [FromQuery] int? idTrabajador,
    [FromQuery] DateTime? FechaDesde,
    [FromQuery] DateTime? FechaHasta,
    [FromQuery] int? idEstadoReserva,
    [FromQuery] int? idtiporeserva)
    {
        _logger.LogInformation("ReservasTrabajadorDisponibles : inicio.");

        try
        {
            var filtro = new ReservaTrabajadorDto
            {
                IdReserva = idReserva ?? 0,
                IdHabitacion = idHabitacion ?? 0,
                IdTrabajador = idTrabajador ?? 0,
                FechaDesde = FechaDesde,
                FechaHasta = FechaHasta,
                IdEstadoReserva = idEstadoReserva ?? 0, // si 0 significa “no filtrar”, tu servicio debe enviarlo como DBNull
                IdTipoReserva = idtiporeserva ?? 0
            };

            var reservas = _reservaService.GetListaReservaTrabajador(filtro);

            if (reservas == null || reservas.Count == 0)
            {
                _logger.LogInformation("ReservasTrabajadorDisponibles : sin resultados.");
                return new NoContentResult();
            }

            _logger.LogInformation($"ReservasTrabajadorDisponibles : {reservas.Count} resultado(s).");
            return Ok(reservas);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ReservasTrabajadorDisponibles : error.");
            return StatusCode(500, e.Message);
        }
    }

        // POST: api/Reservas/CreaReservaTrabajador
        [HttpPost("CreaReservaTrabajador")]
        public ActionResult<object> CreaReservaTrabajador([FromBody] ReservaTrabajadorDto dto)
        {
            _logger.LogInformation("CreaReservaTrabajador : inicio.");

            if (dto == null) return BadRequest("Payload vacío.");

            // Validación mínima para INSERT
            if ((dto.IdHabitacion <= 0 || dto.IdTrabajador <= 0))
                return BadRequest("IdHabitacion e IdTrabajador son obligatorios al crear.");

            try
            {
                var idGenerado = _reservaService.CreaReservaTrabajador(dto);
                if (idGenerado <= 0)
                {
                    _logger.LogWarning("CreaReservaTrabajador : operación no realizada.");
                    return StatusCode(500, "No se pudo crear/editar la reserva.");
                }

                _logger.LogInformation("CreaReservaTrabajador : ok (IdReserva={Id}).", idGenerado);
                return Ok(new { idReserva = idGenerado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreaReservaTrabajador : error.");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Lista los estados de reserva (hot_EstadoReservas).
        /// SP: HOT_ESTADO_RESERVA_LISTAR
        /// </summary>
        [HttpGet("ListarEstadoReserva")]
        [ProducesResponseType(typeof(List<EstadoReservaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<EstadoReservaDto>> Listar()
        {
            try
            {
                var data = _reservaService.GetListaEstadoReserva() ?? new List<EstadoReservaDto>();
                if (data.Count == 0) return NoContent();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EstadoReserva/Listar] Error al listar estados de reserva.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno al listar estados de reserva.");
            }
        }


    }
}

