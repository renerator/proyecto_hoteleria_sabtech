using System;
using System.Collections.Generic;
using DemoBackend.Dto.Calendario;
using DemoBackend.Services.Calendario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DemoBackend.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class CalendarioController : ControllerBase
    {
        private readonly ICalendarioService _service;
        private readonly ILogger<CalendarioController> _logger;
        //cambio 1-12
        public CalendarioController(ICalendarioService service, ILogger<CalendarioController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<CalendarioEventoDto>> Get([FromQuery] int? habitacionId, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            return Ok(_service.GetEventos(habitacionId, desde, hasta));
        }

        [HttpGet("{id:int}")]
        public ActionResult<CalendarioEventoDto> GetById(int id)
        {
            var dto = _service.GetEventos(id,null,null);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public ActionResult Post([FromBody] CalendarioEventoDto dto)
        {
            if (_service.CrearEvento(dto)) return Ok("Evento creado.");
            return StatusCode(500, "No se pudo crear.");
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, [FromBody] CalendarioEventoDto dto)
        {
            if (_service.ActualizarEvento(dto)) return Ok("Evento actualizado.");
            return StatusCode(500, "No se pudo actualizar.");
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            if (_service.EliminarEvento(id)) return Ok("Evento eliminado.");
            return StatusCode(500, "No se pudo eliminar.");
        }

        [HttpGet("resumen")]
        public ActionResult<CalendarioKpiDto> Resumen()
        {
            return Ok(_service.GetKpi());
        }

        [HttpPost("sanitizacion")]
        public ActionResult Sanitizacion([FromBody] CalendarioSanitizacionDto dto)
        {
            if (_service.CrearSanitizacion(dto)) return Ok("Sanitización programada.");
            return StatusCode(500, "No se pudo programar.");
        }

        //[HttpGet("/api/calendario/habitaciones")]
        //public ActionResult<List<CalendarioHabitacionDto>> Habitaciones()
        //{
        //    return Ok(_service..ListarHabitaciones());
        //}

        [HttpPost("/api/calendario/bloqueos")]
        public ActionResult CrearBloqueo([FromBody] CalendarioBloqueoDto dto)
        {
            if (_service.CrearBloqueo(dto)) return Ok("Bloqueo creado.");
            return StatusCode(500, "No se pudo crear el bloqueo.");
        }

        [HttpPost("/api/calendario/mantenimientos")]
        public ActionResult CrearMantenimiento([FromBody] CalendarioMantenimientoDto dto)
        {
            if (_service.CrearMantenimiento(dto)) return Ok("Mantenimiento creado.");
            return StatusCode(500, "No se pudo crear el mantenimiento.");
        }
    }
}
