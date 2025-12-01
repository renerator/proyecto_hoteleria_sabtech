using DemoBackend.Dto.Bodega;
using DemoBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DemoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BodegasController : BaseController
    {
        private readonly IBodegaService _service;
        private readonly ILogger<BodegasController> _logger;
        //cambio 1-12
        public BodegasController(IBodegaService service, ILogger<BodegasController> logger)
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
        [HttpGet("ListaBodegasVigentes")]
        public ActionResult<List<BodegaDto>> ListaBodegasVigentes([FromQuery] int vigencia = 1)
        {
            try
            {
                var data = _service.GetListaBodegaEstado(vigencia);
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
        [HttpPost("CrearBodega")]
        public ActionResult CrearBodega([FromBody] BodegaDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.NombreBodega))
                    return BadRequest("Descripción requerida.");

                var existe = _service.VerificaBodegaPorId(dto);
                if (existe != null && existe.Count > 0)
                    return Ok($"Ya existe la bodega con Id {dto.IdBodega}");

                var grupoOK = _service.CrearBodega(dto);
                if (grupoOK)
                    return Ok(grupoOK + " OK, Datos insertados");
                else
                    return Ok(grupoOK + " Datos no insertados");
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
        [HttpPut("ModificarBodega")]
        public ActionResult ModificarBodega([FromBody] BodegaDto dto)
        {
            try
            {
                if (dto == null || dto.IdBodega == 0) return BadRequest("IdBodega requerido.");
                var existe = _service.VerificaBodegaPorId(dto);
                var grupoOK = _service.ModificarBodega(dto);
                if (existe == null || existe.Count == 0) { return Ok($"No existe la bodega {dto.IdBodega}"); }
                else
                {
                    if (grupoOK)
                    {
                        return Ok(grupoOK + " OK, Datos Modificados");
                    }
                    else
                    {
                        return Ok(grupoOK + " Datos no Modificados");
                    }
                }
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
        [HttpDelete("EliminarBodega")]
        public ActionResult EliminarBodega([FromQuery] int idBodega)
        {
            try
            {
                if (idBodega == 0) return BadRequest("IdBodega requerido.");
                var dto = new BodegaDto { IdBodega = idBodega };
                var existe = _service.VerificaBodegaPorId(dto);
                if (existe == null || existe.Count == 0) return Ok($"No existe la bodega {idBodega}");

                var ok = _service.EliminarBodega(dto);
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
        [HttpGet("BuscarBodegas")]
        public ActionResult<List<BodegaDto>> BuscarBodegas(
            [FromQuery] int? idBodega,
            [FromQuery] string? NombreBodega,
            [FromQuery] int? idEmpresa,
            [FromQuery] int? vigencia)
        {
            try
            {
                var filtro = new BodegaDto
                {
                    IdBodega = idBodega ?? 0,
                    NombreBodega = string.IsNullOrWhiteSpace(NombreBodega) ? null : NombreBodega.Trim(),
                    IdEmpresa = idEmpresa,
                    //Vigencia = vigencia
                };

                var data = _service.BuscaBodegas(filtro);
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
