using DemoBackend.Dto.SolicitudServicio;
using DemoBackend.Services.SolicitudServicio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoBackend.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class SolicitudServicioController : BaseController
    {
        private readonly ISolicitudServicioService _service;
        private readonly ILogger _logger;
        //cambio 1-12
        public SolicitudServicioController(
            ISolicitudServicioService service,
            ILogger<SolicitudServicioController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Busca solicitudes de servicio por filtros opcionales.
        /// </summary>
        /// <param name="idSolicitud">Id de la solicitud</param>
        /// <param name="idHabitacion">Id de la habitación</param>
        /// <param name="idServicio">Id del servicio</param>
        /// <param name="desde">Fecha desde (inclusive)</param>
        /// <param name="hasta">Fecha hasta (inclusive)</param>
        /// <returns>Lista de solicitudes</returns>
        /// <response code="200">OK - Retorna lista de solicitudes</response>
        /// <response code="204">Sin resultados</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("BuscarSolicitudes")]
        public ActionResult<List<SolicitudServicioDto>> BuscarSolicitudes(
            [FromQuery] int? idSolicitud,
            [FromQuery] int? idHabitacion,
            [FromQuery] int? idServicio,
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta)
        {
            _logger.LogInformation("GetBuscarSolicitudes: Inicio de búsqueda.");
            try
            {
                var resultados = _service.Buscar(
                    idSolicitud: idSolicitud,
                    idHabitacion: idHabitacion,
                    idServicio: idServicio,
                    desde: desde,
                    hasta: hasta
                );

                if (resultados == null || resultados.Count == 0)
                {
                    _logger.LogInformation("GetBuscarSolicitudes: Sin resultados.");
                    return NoContent();
                }

                _logger.LogInformation($"GetBuscarSolicitudes: {resultados.Count} registro(s) encontrado(s).");
                return Ok(resultados);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetBuscarSolicitudes: Error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Obtiene una solicitud por su Id.
        /// </summary>
        /// <param name="idSolicitud">Id de la solicitud</param>
        /// <returns>Solicitud encontrada o 204 si no existe</returns>
        [HttpGet("Obtener")]
        public ActionResult<SolicitudServicioDto?> Obtener([FromQuery] int idSolicitud)
        {
            _logger.LogInformation("GetObtenerSolicitud: Inicio.");
            try
            {
                if (idSolicitud <= 0)
                {
                    _logger.LogInformation("GetObtenerSolicitud: idSolicitud inválido.");
                    return BadRequest("Parámetro idSolicitud es requerido y debe ser > 0.");
                }

                var dto = _service.ObtenerPorId(idSolicitud);
                if (dto == null)
                {
                    _logger.LogInformation($"GetObtenerSolicitud: No existe solicitud con Id {idSolicitud}.");
                    return NoContent();
                }

                return Ok(dto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetObtenerSolicitud: Error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpGet("ResumenKPI")]
        public async Task<ActionResult<SolicitudKPIDto>> ObtenerKPI()
        {
            _logger.LogInformation("GetObtenerKPI: Inicio.");
            try
            {
                var dto = await _service.ObtenerKpiAsync();   // 👈 await al Task

                if (dto == null)
                {
                    _logger.LogInformation("GetObtenerKPI: No hay datos de KPI.");
                    return NoContent();
                }

                return Ok(dto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetObtenerKPI: Error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }


        /// <summary>
        /// Crea una nueva solicitud de servicio.
        /// </summary>
        /// <param name="dto">Datos de la solicitud</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">Inserción exitosa o mensaje descriptivo</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpPost("CrearSolicitud")]
        public ActionResult CrearSolicitud([FromBody] SolicitudServicioDto dto)
        {
            _logger.LogInformation("PostCrearSolicitud: Inicio.");
            try
            {
                if (dto == null)
                    return BadRequest("Datos de solicitud vacíos.");

                if (dto.IdHabitacion <= 0 || dto.IdServicio <= 0)
                    return Ok("Status 200: Error de validación (IdHabitacion/IdServicio requeridos).");

                var ok = _service.Crear(dto);
                if (ok)
                    return Ok("Solicitud creada correctamente.");
                else
                    return Ok("No se pudo crear la solicitud.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "PostCrearSolicitud: Error.");
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Modifica una solicitud existente.
        /// </summary>
        /// <param name="dto">Datos de la solicitud a modificar (IdSolicitud requerido)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("ModificarSolicitud")]
        public ActionResult ModificarSolicitud([FromBody] SolicitudServicioDto dto)
        {
            _logger.LogInformation("PutModificarSolicitud: Inicio.");
            try
            {
                if (dto == null || dto.IdSolicitud <= 0)
                    return Ok("Status 200: Error de validación (IdSolicitud requerido).");

                var existe = _service.ObtenerPorId(dto.IdSolicitud);
                if (existe == null)
                    return Ok($"No se puede modificar, no existe la solicitud con Id {dto.IdSolicitud}.");

                var ok = _service.Modificar(dto);
                if (ok)
                    return Ok("Solicitud modificada correctamente.");
                else
                    return Ok("No se pudo modificar la solicitud.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "PutModificarSolicitud: Error.");
                return StatusCode(500, e.Message);
            }
        }
        // En SolicitudServicioController
        [HttpGet("ListaSolicitudesVigentes")]
        public ActionResult<List<SolicitudServicioDto>> ListaSolicitudesVigentes(
     [FromQuery] int IdEstado = 1,
     [FromQuery] DateTime? fechaInicio = null,
     [FromQuery] DateTime? fechaFin = null)
        {
            try
            {
                // ahora el servicio recibe también el rango de fechas
                var data = _service.GetListaSolicitudServicioEstado(IdEstado, fechaInicio, fechaFin);

                if (data == null || data.Count == 0)
                    return NoContent();

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Vigentes: error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Elimina una solicitud por Id.
        /// </summary>
        /// <param name="idSolicitud">Id de la solicitud</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("EliminarSolicitud")]
        public ActionResult EliminarSolicitud([FromQuery] int idSolicitud)
        {
            _logger.LogInformation("DelEliminarSolicitud: Inicio.");
            try
            {
                if (idSolicitud <= 0)
                    return Ok("Status 200: Error de validación (idSolicitud requerido).");

                var existe = _service.ObtenerPorId(idSolicitud);
                if (existe == null)
                    return Ok($"No se puede eliminar, no existe la solicitud con Id {idSolicitud}.");

                var ok = _service.Eliminar(idSolicitud);
                if (ok)
                    return Ok("Solicitud eliminada correctamente.");
                else
                    return Ok("No se pudo eliminar la solicitud.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "DelEliminarSolicitud: Error.");
                return StatusCode(500, e.Message);
            }
        }
    }
}
