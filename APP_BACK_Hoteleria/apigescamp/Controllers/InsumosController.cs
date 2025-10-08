using DemoBackend.Dto.Insumos;
using DemoBackend.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DemoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsumosController : ControllerBase
    {
        private readonly IInsumoService _service;
        private readonly ILogger<InsumosController> _logger;

        public InsumosController(IInsumoService service, ILogger<InsumosController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Servicio que devuelve una lista de reservas según los filtros ingresados.
        /// </summary>
        /// <param name="BodegaDto">Objeto con los filtros opcionales de búsqueda.</param>
        /// <returns>Lista de reservas que cumplen los criterios.</returns>
        /// <response code="200">OK - Retorna lista de reservas.</response>
        /// <response code="204">Sin resultados.</response>
        /// <response code="400">Solicitud inválida.</response>
        /// <response code="401">No autorizado.</response>
        /// <response code="403">Acceso denegado.</response>
        /// <response code="500">Error interno.</response>
        [HttpGet("ListainsumosVigentes")]
        public ActionResult<List<InsumoDto>> ListaInsumosVigentes([FromQuery] int vigencia = 1)
        {
            try
            {
                var data = _service.GetListaInsumoEstado(vigencia);
                if (data == null || data.Count == 0) return NoContent();
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Vigentes: error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Servicio que devuelve una lista de reservas según los filtros ingresados.
        /// </summary>
        /// <param name="BodegaDto">Objeto con los filtros opcionales de búsqueda.</param>
        /// <returns>Lista de reservas que cumplen los criterios.</returns>
        /// <response code="200">OK - Retorna lista de reservas.</response>
        /// <response code="204">Sin resultados.</response>
        /// <response code="400">Solicitud inválida.</response>
        /// <response code="401">No autorizado.</response>
        /// <response code="403">Acceso denegado.</response>
        /// <response code="500">Error interno.</response>
        [HttpPost("CrearInsumos")]
        public ActionResult CrearInsumos([FromBody] InsumoDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.NombreInsumo))
                    return BadRequest("Nombre requerido.");

                var existe = _service.VerificaInsumoPorId(dto);
                if (existe != null && existe.Count > 0)
                    return Ok($"Ya existe el insumo con Id {dto.IdInsumo}");

                var ok = _service.CrearInsumo(dto);
                if (ok)
                    return Ok(ok + " OK, Datos insertados");
                else
                    return Ok(ok + " Datos no insertados");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Crear: error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Servicio que devuelve una lista de reservas según los filtros ingresados.
        /// </summary>
        /// <param name="BodegaDto">Objeto con los filtros opcionales de búsqueda.</param>
        /// <returns>Lista de reservas que cumplen los criterios.</returns>
        /// <response code="200">OK - Retorna lista de reservas.</response>
        /// <response code="204">Sin resultados.</response>
        /// <response code="400">Solicitud inválida.</response>
        /// <response code="401">No autorizado.</response>
        /// <response code="403">Acceso denegado.</response>
        /// <response code="500">Error interno.</response>
        [HttpPut("ModificarInsumos")]
        public ActionResult ModificarInsumos([FromBody] InsumoDto dto)
        {
            try
            {
                if (dto == null || dto.IdInsumo == 0) return BadRequest("IdInsumo requerido.");
                var existe = _service.VerificaInsumoPorId(dto);
                var ok = _service.ModificarInsumo(dto);

                if (existe == null || existe.Count == 0)
                    return Ok($"No existe el insumo {dto.IdInsumo}");

                if (ok)
                    return Ok(ok + " OK, Datos Modificados");
                else
                    return Ok(ok + " Datos no Modificados");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Modificar: error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Servicio que devuelve una lista de reservas según los filtros ingresados.
        /// </summary>
        /// <param name="BodegaDto">Objeto con los filtros opcionales de búsqueda.</param>
        /// <returns>Lista de reservas que cumplen los criterios.</returns>
        /// <response code="200">OK - Retorna lista de reservas.</response>
        /// <response code="204">Sin resultados.</response>
        /// <response code="400">Solicitud inválida.</response>
        /// <response code="401">No autorizado.</response>
        /// <response code="403">Acceso denegado.</response>
        /// <response code="500">Error interno.</response>
        [HttpDelete("EliminarInsumos")]
        public ActionResult EliminarInsumos([FromQuery] int idInsumo)
        {
            try
            {
                if (idInsumo == 0) return BadRequest("IdInsumo requerido.");

                var dto = new InsumoDto { IdInsumo = idInsumo };
                var existe = _service.VerificaInsumoPorId(dto);
                if (existe == null || existe.Count == 0)
                    return Ok($"No existe el insumo {idInsumo}");

                var ok = _service.EliminarInsumo(dto);
                if (ok)
                    return Ok(ok + " OK, Datos eliminados");
                else
                    return Ok(ok + " Datos no eliminados");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Eliminar: error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Servicio que devuelve una lista de reservas según los filtros ingresados.
        /// </summary>
        /// <param name="BodegaDto">Objeto con los filtros opcionales de búsqueda.</param>
        /// <returns>Lista de reservas que cumplen los criterios.</returns>
        /// <response code="200">OK - Retorna lista de reservas.</response>
        /// <response code="204">Sin resultados.</response>
        /// <response code="400">Solicitud inválida.</response>
        /// <response code="401">No autorizado.</response>
        /// <response code="403">Acceso denegado.</response>
        /// <response code="500">Error interno.</response>
        [HttpGet("BuscarInsumos")]
        public ActionResult<List<InsumoDto>> BuscarInsumos(
            [FromQuery] int? idInsumo,
            [FromQuery] string? NombreInsumo,
            [FromQuery] int? StockMinimo,
            [FromQuery] int? idBodega)
            
        {
            try
            {
                var filtro = new InsumoDto
                {
                    IdInsumo = idInsumo ?? 0,
                    NombreInsumo = string.IsNullOrWhiteSpace(NombreInsumo) ? null : NombreInsumo.Trim(),
                    StockMinimo = StockMinimo,
                    IdBodega = idBodega,
                    //Vigencia = vigencia
                };

                var data = _service.BuscaInsumos(filtro);
                if (data == null || data.Count == 0) return NoContent();
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Buscar: error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
