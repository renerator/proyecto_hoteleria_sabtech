using DemoBackend.Dto.OrdenTrabajo;
using DemoBackend.Services.OrdenTrabajo;
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
    public class OrdenesTrabajoController : BaseController
    {
        private readonly IOrdenTrabajoService _service;
        private readonly ILogger _logger;

        public OrdenesTrabajoController(
            IOrdenTrabajoService service,
            ILogger<OrdenesTrabajoController> logger)
        {
            _service = service;
            _logger  = logger;
        }

        /// <summary>
        /// Búsqueda de órdenes de trabajo por filtros opcionales.
        /// </summary>
        [HttpGet("BuscarOrdenes")]
        public ActionResult<List<OrdenTrabajoDto>> BuscarOrdenes(
            [FromQuery] int? idOrdenTrabajo,
            [FromQuery] int? idHabitacion,
            [FromQuery] string? numeroOT,
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta)
        {
            _logger.LogInformation("GetBuscarOrdenes: Inicio.");
            try
            {
                var resultados = _service.Buscar(
                    idOrdenTrabajo: idOrdenTrabajo,
                    idHabitacion:   idHabitacion,
                    numeroOT:       numeroOT,
                    desde:          desde,
                    hasta:          hasta
                );

                if (resultados == null || resultados.Count == 0)
                {
                    _logger.LogInformation("GetBuscarOrdenes: Sin resultados.");
                    return NoContent();
                }

                _logger.LogInformation($"GetBuscarOrdenes: {resultados.Count} registro(s) encontrado(s).");
                return Ok(resultados);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetBuscarOrdenes: Error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Obtiene una OT por su Id.
        /// </summary>
        [HttpGet("Obtener")]
        public ActionResult<OrdenTrabajoDto?> Obtener([FromQuery] int idOrdenTrabajo)
        {
            _logger.LogInformation("GetObtenerOrden: Inicio.");
            try
            {
                if (idOrdenTrabajo <= 0)
                    return BadRequest("Parámetro idOrdenTrabajo es requerido y debe ser > 0.");

                var dto = _service.ObtenerPorId(idOrdenTrabajo);
                if (dto == null) return NoContent();

                return Ok(dto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetObtenerOrden: Error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Crea una nueva OT.
        /// </summary>
        [HttpPost("CrearOrden")]
        public ActionResult CrearOrden([FromBody] OrdenTrabajoDto dto)
        {
            _logger.LogInformation("PostCrearOrden: Inicio.");
            try
            {
                if (dto == null) return BadRequest("Datos de OT vacíos.");
                if (dto.IdHabitacion <= 0) return Ok("Status 200: Error de validación (IdHabitacion requerido).");
                if (string.IsNullOrWhiteSpace(dto.NumeroOT)) return Ok("Status 200: Error de validación (NumeroOT requerido).");

                var ok = _service.Crear(dto);
                return Ok(ok ? "Orden creada correctamente." : "No se pudo crear la orden.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "PostCrearOrden: Error.");
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Modifica una OT existente.
        /// </summary>
        [HttpPut("ModificarOrden")]
        public ActionResult ModificarOrden([FromBody] OrdenTrabajoDto dto)
        {
            _logger.LogInformation("PutModificarOrden: Inicio.");
            try
            {
                if (dto == null || dto.IdOrdenTrabajo <= 0)
                    return Ok("Status 200: Error de validación (IdOrdenTrabajo requerido).");

                var existe = _service.ObtenerPorId(dto.IdOrdenTrabajo);
                if (existe == null)
                    return Ok($"No se puede modificar, no existe la orden con Id {dto.IdOrdenTrabajo}.");

                var ok = _service.Modificar(dto);
                return Ok(ok ? "Orden modificada correctamente." : "No se pudo modificar la orden.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "PutModificarOrden: Error.");
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Elimina una OT por Id.
        /// </summary>
        [HttpDelete("EliminarOrden")]
        public ActionResult EliminarOrden([FromQuery] int idOrdenTrabajo)
        {
            _logger.LogInformation("DelEliminarOrden: Inicio.");
            try
            {
                if (idOrdenTrabajo <= 0)
                    return Ok("Status 200: Error de validación (idOrdenTrabajo requerido).");

                var existe = _service.ObtenerPorId(idOrdenTrabajo);
                if (existe == null)
                    return Ok($"No se puede eliminar, no existe la orden con Id {idOrdenTrabajo}.");

                var ok = _service.Eliminar(idOrdenTrabajo);
                return Ok(ok ? "Orden eliminada correctamente." : "No se pudo eliminar la orden.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "DelEliminarOrden: Error.");
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Lista de OTs vigentes (por estado/vigencia).
        /// </summary>
        [HttpGet("ListaOrdenesVigentes")]
        public ActionResult<List<OrdenTrabajoDto>> ListaOrdenesVigentes([FromQuery] int vigencia = 1)
        {
            try
            {
                var data = _service.GetListaOrdenTrabajoEstado(vigencia);
                if (data == null || data.Count == 0) return NoContent();
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Vigentes: error inesperado.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
