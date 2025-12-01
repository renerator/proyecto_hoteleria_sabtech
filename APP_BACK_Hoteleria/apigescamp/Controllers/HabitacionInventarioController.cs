using DemoBackend.Dto.HabitacionInventario;
using DemoBackend.Services.HabitacionInventario;
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
    public class HabitacionInventarioController : BaseController
    {
        private readonly ILogger<HabitacionInventarioController> _logger;
        private readonly IHabitacionInventarioService _service;
        //cambio 1-12
        public HabitacionInventarioController(
            ILogger<HabitacionInventarioController> logger,
            IHabitacionInventarioService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Servicio que retorna el listado de las Habitacion 
        /// </summary>
        /// <returns>lista Habitacion</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("ListarHabitacionInsumo")]
        public ActionResult<List<HabitacionInventarioDto>> ListarHabitacionInsumo(int vigencia)
        {
            _logger.LogInformation("ListarHabitacionInsumo: inicio.");
            try
            {
                var lista = _service.GetListaHabitacionInsumoEstado(vigencia);

                if (lista == null || lista.Count == 0)
                {
                    _logger.LogInformation("ListarHabitacionInsumo: sin resultados.");
                    return new NoContentResult();
                }

                _logger.LogInformation("ListarHabitacionInsumo: {Count} registros.", lista.Count);
                return Ok(lista);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ListarHabitacionInsumo: error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Crea una relación Habitación–Insumo.
        /// </summary>
        ///        
        /// <returns>lista Habitacion</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpPost("CrearHabitacionInsumo")]
        public IActionResult Crear([FromBody] HabitacionInventarioDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("CrearHabitacionInsumo: body nulo.");
                    return BadRequest("Body inválido.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("CrearHabitacionInsumo: ModelState inválido.");
                    return BadRequest(ModelState);
                }

                if (dto.IdHabitacion <= 0 || dto.IdInventario <= 0)
                {
                    _logger.LogWarning("CrearHabitacionInsumo: parámetros inválidos (IdHabitacion/IdInsumo).");
                    return BadRequest("Parámetros inválidos.");
                }

                var ok = _service.CrearHabitacionInsumo(dto);
                if (ok)
                {
                    _logger.LogInformation("CrearHabitacionInsumo: creado correctamente.");
                    return Ok("Creado");
                }

                _logger.LogError("CrearHabitacionInsumo: fallo en capa de servicio.");
                return StatusCode(500, "Error al crear insumo.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CrearHabitacionInsumo: error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Modifica una relación Habitación–Insumo.
        /// </summary>
        /// <returns>lista Habitacion</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpPut("ModificarHabitacionInsumo")]
        public IActionResult Modificar([FromBody] HabitacionInventarioDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("ModificarHabitacionInsumo: body nulo.");
                    return BadRequest("Body inválido.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModificarHabitacionInsumo: ModelState inválido.");
                    return BadRequest(ModelState);
                }

                if (dto.IdHabitacionInventario <= 0)
                {
                    _logger.LogWarning("ModificarHabitacionInsumo: falta IdHabitacionInsumo.");
                    return BadRequest("Falta idHabitacionInsumo.");
                }

        
                var ok = _service.ModificarHabitacionInsumo(dto);
                if (ok)
                {
                    _logger.LogInformation("ModificarHabitacionInsumo: modificado correctamente.");
                    return Ok("Modificado");
                }

                _logger.LogError("ModificarHabitacionInsumo: fallo en capa de servicio.");
                return StatusCode(500, "Error al modificar.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ModificarHabitacionInsumo: error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Elimina una relación Habitación–Insumo por ID.
        /// </summary>      
        /// <returns>lista Habitacion</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpDelete("EliminarHabitacionInsumo")]
        public IActionResult Eliminar(int idHabitacionInventario)
        {
            try
            {
                if (idHabitacionInventario <= 0)
                {
                    _logger.LogWarning("EliminarHabitacionInsumo: id inválido ({Id}).", idHabitacionInventario);
                    return BadRequest("Parámetro inválido.");
                }

                var ok = _service.EliminarHabitacionInsumo(idHabitacionInventario);
                if (ok)
                {
                    _logger.LogInformation("EliminarHabitacionInsumo: eliminado correctamente ({Id}).", idHabitacionInventario);
                    return Ok("Eliminado");
                }

                _logger.LogError("EliminarHabitacionInsumo: fallo en capa de servicio ({Id}).", idHabitacionInventario);
                return StatusCode(500, "Error al eliminar.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "EliminarHabitacionInsumo: error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
