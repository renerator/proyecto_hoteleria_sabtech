using DemoBackend.Dto.Mantenedores;
using DemoBackend.Dto.Reserva;
using DemoBackend.Models.Reserva;
using DemoBackend.Services;
using DemoBackend.Services.Reserva;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

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
        public ActionResult<List<ReservaDto>> ReservasDisponibles(int vigencia)
        {
            _logger.LogInformation("GetListaReservas : Inicio proceso lista de Reservas");
            try
            {
                var reservas = _reservaService.GetListaReservaEstado(vigencia);
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
    }
}
