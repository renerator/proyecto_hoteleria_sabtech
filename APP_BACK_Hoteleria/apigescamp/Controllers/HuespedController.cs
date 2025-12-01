using DemoBackend.Dto.Huesped;
using DemoBackend.Services.Huesped;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DemoBackend.Controllers
{
    [ApiController]
    [Route("api/Huesped")]
    [Authorize]
    public class HuespedController : BaseController
    {
        private readonly IHuespedService _huespedService;
        private readonly ILogger _logger;
        //cambio 1-12
        public HuespedController(
            IHuespedService huespedService,
            ILogger<HuespedController> logger)
        {
            _huespedService = huespedService;
            _logger = logger;
        }

        #region RECLAMOS HUESPED

        /// <summary>
        /// Crea un reclamo / sugerencia / felicitación de huésped.
        /// </summary>
        [HttpPost("CrearReclamo")]
        public async Task<IActionResult> Crear([FromBody] ReclamoSolicitudDto dto)
        {
            _logger.LogInformation("POST api/ReclamosHuesped/Crear : inicio.");

            if (dto == null)
                return BadRequest("Datos vacíos.");

            try
            {
                if (dto.IdTipoSolicitudHuesped <= 0 ||
                    dto.IdCategoriaHuesped <= 0 ||
                    dto.IdPrioridad <= 0 ||
                    string.IsNullOrWhiteSpace(dto.Asunto) ||
                    string.IsNullOrWhiteSpace(dto.Descripcion) ||
                    string.IsNullOrWhiteSpace(dto.Email))
                {
                    return BadRequest("Faltan campos obligatorios.");
                }

                if (dto.Fecha == default)
                    dto.Fecha = DateTime.Now;

                if (dto.IdEstado == 0)
                    dto.IdEstado = 1; // pendiente

                if (string.IsNullOrWhiteSpace(dto.Estado))
                    dto.Estado = "pendiente";

                // TODO: obtener IdUsuarioActualizacion desde Claims si corresponde
                // if (dto.IdUsuarioActualizacion == 0) { ... }

                var ok = await _huespedService.CrearReclamoHuespedAsync(dto, null);
                if (!ok)
                {
                    _logger.LogWarning("POST api/ReclamosHuesped/Crear : no se pudo crear el reclamo.");
                    return StatusCode(500, "No se pudo crear el reclamo.");
                }

                _logger.LogInformation("POST api/ReclamosHuesped/Crear : OK.");
                return Ok(new { ok = true, message = "Reclamo creado correctamente." });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "POST api/ReclamosHuesped/Crear : error.");
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Obtiene el detalle de una solicitud por id.</summary>
        [HttpGet("ObtenerReclamo/{idReclamoHuesped:int}")]
        public ActionResult<ReclamoSolicitudDto> Reclamo(int idReclamoHuesped)
        {
            if (idReclamoHuesped <= 0)
                return BadRequest("Id inválido.");

            try
            {
                var dto = _huespedService.ObtenerReclamoHuespedPorId(idReclamoHuesped);
                if (dto == null)
                    return NoContent();   // o NotFound()

                return Ok(dto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GET api/ReclamosHuesped/Reclamo/{id}");
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Lista los reclamos / sugerencias de huésped.</summary>
        [HttpGet("ListarReclamos")]
        public async Task<ActionResult<List<ReclamoSolicitudDto>>> Listar()
        {
            _logger.LogInformation("GET api/ReclamosHuesped/Listar : inicio.");

            try
            {
                var lista = await _huespedService.ListarReclamosHuespedAsync(null);

                if (lista == null || lista.Count == 0)
                {
                    _logger.LogInformation("GET api/ReclamosHuesped/Listar : sin resultados.");
                    return NoContent();
                }

                _logger.LogInformation($"GET api/ReclamosHuesped/Listar : {lista.Count} resultado(s).");
                return Ok(lista);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GET api/ReclamosHuesped/Listar : error.");
                return StatusCode(500, e.Message);
            }
        }

        #endregion

        #region RESERVAS HUESPED

        [HttpGet("BuscarReserva")]
        public ActionResult<List<ReservaHuespedDto>> Buscar([FromQuery] ReservaHuespedDto filtro)
        {
            var lista = _huespedService.Buscar(filtro) ?? new List<ReservaHuespedDto>();
            if (lista.Count == 0) return NoContent();
            return Ok(lista);
        }

        [HttpGet("ObtenerReserva/{idReserva:int}")]
        public ActionResult<ReservaHuespedDto> ObtenerReserva(int idReserva)
        {
            var dto = _huespedService.ObtenerPorId(idReserva);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost("RegistrarEncuesta")]
        public IActionResult RegistrarEncuesta([FromBody] EncuestaSatisfaccionDto dto)
        {
            if (dto == null)
                return BadRequest("Encuesta inválida.");

            var ok = _huespedService.RegistrarEncuesta(dto);
            if (!ok)
                return StatusCode(500, "No se pudo registrar la encuesta.");

            return NoContent();
        }

        [HttpPost("CrearReserva")]
        public ActionResult CrearReserva([FromBody] ReservaHuespedDto dto)
        {
            var ok = _huespedService.Crear(dto);
            if (!ok) return StatusCode(500, "No se pudo crear la reserva.");
            return Ok();
        }

        [HttpPut("ActualizarReserva/{id:int}")]
        public ActionResult ActualizarReserva(int id, [FromBody] ReservaHuespedDto dto)
        {
            dto.IdReserva = id;
            var ok = _huespedService.Actualizar(dto);
            if (!ok) return StatusCode(500, "No se pudo actualizar la reserva.");
            return Ok();
        }

        [HttpDelete("EliminarReserva/{idReserva:int}")]
        public IActionResult EliminarReserva(int idReserva)
        {
            var ok = _huespedService.Eliminar(idReserva);
            if (!ok)
                return StatusCode(500, "No se pudo eliminar la reserva.");

            return NoContent();
        }

        #endregion

        #region SERVICIOS HUESPED (NUEVO)

        /// <summary>
        /// Busca solicitudes de servicio del huésped según filtros.
        /// POST /api/Huesped/BuscarServicios
        /// </summary>
        // GET: /api/Huesped/BuscarServicios
        [HttpGet("BuscarServicios")]
        public ActionResult<List<ServicioHuespedDto>> BuscarServicios(
     [FromQuery] int? idEstado,
     [FromQuery] string nombreServicio,
     [FromQuery] string texto,
     [FromQuery] DateTime? desde,
     [FromQuery] DateTime? hasta)
        {
            try
            {
                var filtro = new ServicioHuespedDto
                {
                    FiltroIdEstado = idEstado,
                    FiltroNombreServicio = nombreServicio,
                    FiltroTexto = texto,
                    FiltroDesde = desde,
                    FiltroHasta = hasta
                };

                // 👇 OJO: SIN await, porque el método es síncrono
                var lista = _huespedService.BuscarServiciosHuesped(filtro);

                if (lista == null || lista.Count == 0)
                    return NoContent();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[HuespedController.BuscarServicios] " + ex);
                return StatusCode(500, "Error al listar servicios del huésped.");
            }
        }


        /// <summary>
        /// Obtiene una solicitud de servicio de huésped por Id.
        /// GET /api/Huesped/Servicio/5
        /// </summary>
        [HttpGet("Servicio/{id:int}")]
        public ActionResult<ServicioHuespedDto> Servicio(int id)
        {
            if (id <= 0)
                return BadRequest("Id inválido.");

            try
            {
                var dto = _huespedService.ObtenerServicioHuespedPorId(id);
                if (dto == null)
                    return NotFound();

                return Ok(dto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GET api/Huesped/Servicio/{id} : error.");
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Crea una nueva solicitud de servicio de huésped.
        /// POST /api/Huesped/CrearServicio
        /// </summary>
        [HttpPost("CrearServicio")]
        public IActionResult CrearServicioHuesped([FromBody] ServicioHuespedDto dto)
        {
            _logger.LogInformation("POST api/Huesped/CrearServicio : inicio.");

            if (dto == null)
                return BadRequest("Datos requeridos.");

            try
            {
                // Defaults simples
                if (!dto.IdEstado.HasValue || dto.IdEstado.Value == 0)
                {
                    dto.IdEstado = 1;               // Pendiente
                    dto.Estado = dto.Estado ?? "Pendiente";
                }

                if (dto.FechaSolicitud == default)
                    dto.FechaSolicitud = DateTime.Now;

                var nuevoId = _huespedService.CrearServicioHuesped(dto);
                if (nuevoId <= 0)
                {
                    _logger.LogWarning("POST api/Huesped/CrearServicio : no se pudo crear la solicitud.");
                    return StatusCode(500, "No se pudo crear la solicitud de servicio.");
                }

                _logger.LogInformation("POST api/Huesped/CrearServicio : OK (Id={Id}).", nuevoId);
                return Ok(new { ok = true, id = nuevoId });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "POST api/Huesped/CrearServicio : error.");
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Actualiza una solicitud de servicio de huésped.
        /// PUT /api/Huesped/ActualizarServicio
        /// </summary>
        [HttpPut("ActualizarServicio")]
        public IActionResult ActualizarServicioHuesped([FromBody] ServicioHuespedDto dto)
        {
            if (dto == null || dto.IdSolicitudServicio <= 0)
                return BadRequest("Id de solicitud requerido.");

            try
            {
                var ok = _huespedService.ActualizarServicioHuesped(dto);
                if (!ok)
                    return StatusCode(500, "No se pudo actualizar la solicitud de servicio.");

                return Ok(new { ok = true });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "PUT api/Huesped/ActualizarServicio : error.");
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Elimina (o da de baja) una solicitud de servicio de huésped.
        /// DELETE /api/Huesped/EliminarServicio/5
        /// </summary>
        [HttpDelete("EliminarServicio/{id:int}")]
        public IActionResult EliminarServicioHuesped(int id)
        {
            if (id <= 0)
                return BadRequest("Id inválido.");

            try
            {
                var ok = _huespedService.EliminarServicioHuesped(id);
                if (!ok)
                    return StatusCode(500, "No se pudo eliminar la solicitud de servicio.");

                // Igual que en reservas: 204 sin contenido
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "DELETE api/Huesped/EliminarServicio/{id} : error.");
                return StatusCode(500, e.Message);
            }
        }

        #endregion
    }
}
