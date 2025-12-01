using System.Collections.Generic;
using DemoBackend.Dto.Dotaciones;
using DemoBackend.Services.Dotaciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoBackend.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class DotacionesController : ControllerBase
    {
        private readonly IDotacionesService _service;
        //cambio 1-12
        public DotacionesController(IDotacionesService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<DotacionDto>> Get([FromQuery] string? criterio = null)
        {
            return Ok(_service.GetDotaciones(criterio));
        }

        [HttpGet("resumen")]
        public ActionResult<DotacionKpiDto> Resumen()
        {
            return Ok(_service.GetKpi());
        }

        [HttpGet("{id:int}")]
        public ActionResult<DotacionDto> GetById(int id)
        {
            var dto = _service.GetDotacion(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public ActionResult Post([FromBody] DotacionDto dto)
        {
            if (_service.CrearDotacion(dto)) return Ok("Dotación creada.");
            return StatusCode(500, "No se pudo crear.");
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, [FromBody] DotacionDto dto)
        {
            if (_service.ActualizarDotacion(dto)) return Ok("Dotación actualizada.");
            return StatusCode(500, "No se pudo actualizar.");
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            if (_service.EliminarDotacion(id)) return Ok("Dotación eliminada.");
            return StatusCode(500, "No se pudo eliminar.");
        }
    }
}
