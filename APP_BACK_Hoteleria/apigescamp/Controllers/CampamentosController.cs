using DemoBackend.Dto.Campamentos;
using DemoBackend.Services.Campamentos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DemoBackend.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class CampamentosController : ControllerBase
    {
        private readonly ICampamentosService _service;
        //cambio 1-12
        public CampamentosController(ICampamentosService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<CampamentoDto>> Get()
        {
            return Ok(_service.GetCampamentos());
        }

        [HttpGet("resumen")]
        public ActionResult<CampamentoKpiDto> Resumen()
        {
            return Ok(_service.GetKpi());
        }

        [HttpGet("{IdCampamento:int}")]
        public ActionResult<CampamentoDto> GetById(int IdCampamento)
        {
            var dto = _service.GetCampamento(IdCampamento);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost ("CrearCampamento")]
        public ActionResult Post([FromBody] CampamentoDto dto)
        {
            if (_service.CrearCampamento(dto)) return Ok("Campamento creado.");
            return StatusCode(500, "No se pudo crear.");
        }

        [HttpPut("EditarCampamento/{IdCampamento:int}")]
        public ActionResult Put(int IdCampamento, [FromBody] CampamentoDto dto)
        {
            if (_service.ActualizarCampamento(dto)) return Ok("Campamento actualizado.");
            return StatusCode(500, "No se pudo actualizar.");
        }

        [HttpDelete("EliminarCampamento/{IdCampamento:int}")]
        public ActionResult Delete(int IdCampamento)
        {
            if (_service.EliminarCampamento(IdCampamento)) return Ok("Campamento eliminado.");
            return StatusCode(500, "No se pudo eliminar.");
        }

        // GET /api/Campamentos/combo?soloActivos=true&filtro=sur
        [HttpGet("combo")]
        [ProducesResponseType(typeof(List<CampamentoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Combo([FromQuery] bool? soloActivos = true, [FromQuery] string? filtro = null)
        {
            var data = _service.ListarCombo(soloActivos, filtro) ?? new List<CampamentoDto>();
            if (data.Count == 0) return NoContent();
            return Ok(data);
        }
    }
}
