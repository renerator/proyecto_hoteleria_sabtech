
using DemoBackend.Dto.Servicio; // Si tu ServicioDto está aquí, déjalo; si no, ajusta el using
 // Quita si no aplica
using DemoBackend.Services;
using DemoBackend.Services.Servicio; // Quita si no aplica
using DemoBackend.Models.Servicio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DemoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiciosHuespedController : BaseController
    {
        private readonly IServicioService _servicioService;
        private readonly ILogger<ServicioController> _logger;

        public ServiciosHuespedController(IServicioService servicioService, ILogger<ServicioController> logger)
        {
            _logger = logger;
            _servicioService = servicioService;
        }

        /// <summary>
        /// Retorna el listado de servicios según vigencia/estado.
        /// </summary>
        /// <param name="vigencia">1=vigente/activo; 0=No vigente (ajusta a tu dominio)</param>
        /// <returns>Lista de servicios</returns>
        /// <response code="200">OK con lista</response>
        /// <response code="204">Sin datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("ListarServicios")]
        public ActionResult<List<ServicioDto>> ListaServicios(int vigencia)
        {
            _logger.LogInformation("GET ServiciosPendientes: inicio.");
            try
            {
                var servicios = _servicioService.GetListaServicioEstado(vigencia);
                if (servicios == null || servicios.Count == 0)
                {
                    _logger.LogInformation("GET ServiciosPendientes: sin resultados.");
                    return NoContent();
                }

                _logger.LogInformation("GET ServiciosPendientes: {Count} registros.", servicios.Count);
                return Ok(servicios);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GET ServiciosPendientes: error.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Crea un servicio.
        /// </summary>
        /// <response code="201">Creado</response>
        /// <response code="400">Solicitud inválida</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="409">Conflicto: ya existe</response>
        /// <response code="500">Error interno</response>
        [HttpPost("CrearServicio")]
        public IActionResult CrearServicio([FromBody] ServicioDto servicioDto)
        {
            try
            {
                if (servicioDto == null)
                    return BadRequest("Body vacío.");

                // Valida campos mínimos (ajusta a tu dominio)
                if (string.IsNullOrWhiteSpace(servicioDto.NombreServicio) && (servicioDto.IdTipoServicio==0))
                    return BadRequest("Faltan campos obligatorios.");

                var existentes = _servicioService.VerificaServicioPorId(servicioDto);
                if (existentes != null && existentes.Count > 0)
                    return Conflict($"Ya existe el servicio con Id {servicioDto.IdServicio}.");

                var ok = _servicioService.CrearServicio(servicioDto);
                if (ok)
                    return StatusCode(201, "Servicio creado correctamente.");
                return BadRequest("No se pudo crear el servicio.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "POST SolicitarServicio: error.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Confirma un servicio.
        /// </summary>
        /// <response code="200">Confirmado</response>
        /// <response code="400">Solicitud inválida</response>
        /// <response code="404">No encontrado</response>
        /// <response code="500">Error interno</response>
        //[HttpPost("ConfirmarServicio")]
        //public IActionResult ConfirmarServicio([FromBody] ServicioDto servicioDto)
        //{
        //    try
        //    {
        //        if (servicioDto == null || servicioDto.IdServicio == 0)
        //            return BadRequest("IdServicio es obligatorio.");

        //        var existe = _servicioService.VerificaServicioPorId(servicioDto);
        //        if (existe == null || existe.Count == 0)
        //            return NotFound($"No existe el servicio con Id {servicioDto.IdServicio}.");

        //        var ok = _servicioService.ModificarServicio(servicioDto); // Debe marcar estado confirmado
        //        return ok ? Ok("Servicio confirmado correctamente.")
        //                  : BadRequest("No se pudo confirmar el servicio.");
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "POST ConfirmarServicio: error.");
        //        return StatusCode(500, "Error interno del servidor.");
        //    }
        //}

        /// <summary>
        /// Modifica un servicio.
        /// </summary>
        /// <response code="200">Modificado</response>
        /// <response code="400">Solicitud inválida</response>
        /// <response code="404">No encontrado</response>
        /// <response code="500">Error interno</response>
        [HttpPut("ModificarServicio")]
        public IActionResult ModificarServicio([FromBody] ServicioDto servicioDto)
        {
            try
            {
                if (servicioDto == null || servicioDto.IdServicio == 0)
                    return BadRequest("IdServicio es obligatorio.");

                var existe = _servicioService.VerificaServicioPorId(servicioDto);
                if (existe == null || existe.Count == 0)
                    return NotFound($"No existe el servicio con Id {servicioDto.IdServicio}.");

                var ok = _servicioService.ModificarServicio(servicioDto);
                return ok ? Ok("Servicio modificado correctamente.")
                          : BadRequest("No se pudo modificar el servicio.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "PUT ModificarServicio: error.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Elimina (o da de baja) un servicio por Id.
        /// </summary>
        /// <param name="idServicio">Id del servicio</param>
        /// <response code="200">Eliminado</response>
        /// <response code="400">Solicitud inválida</response>
        /// <response code="404">No encontrado</response>
        /// <response code="500">Error interno</response>
        [HttpDelete("EliminarServicio")]
        public IActionResult EliminarServicio([FromQuery] int idServicio)
        {
            try
            {
                if (idServicio <= 0)
                    return BadRequest("IdServicio es obligatorio.");

                var dto = new ServicioDto { IdServicio = idServicio };
                var existe = _servicioService.VerificaServicioPorId(dto);
                if (existe == null || existe.Count == 0)
                    return NotFound($"No existe el servicio con Id {idServicio}.");

                var ok = _servicioService.EliminarServicio(dto);
                return ok ? Ok("Servicio eliminado correctamente.")
                          : BadRequest("No se pudo eliminar el servicio.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "DELETE EliminarServicio: error.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
