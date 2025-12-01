using System.Collections.Generic;
using DemoBackend.Dto.Contratos;
using DemoBackend.Services.Contratos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoBackend.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class ContratosController : ControllerBase
    {
        private readonly IContratosService _service;
        //cambio 1-12
        public ContratosController(IContratosService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<ContratoDto>> Get([FromQuery] string? criterio = null)
        {
            return Ok(_service.GetContratos(criterio));
        }

        [HttpGet("resumen")]
        public ActionResult<ContratoKpiDto> Resumen()
        {
            return Ok(_service.GetKpi());
        }

        [HttpGet("{id:int}")]
        public ActionResult<ContratoDto> GetById(int id)
        {
            var dto = _service.GetContrato(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost ("CrearContrato/")]
        public ActionResult Post([FromBody] ContratoDto dto)
        {
            if (_service.CrearContrato(dto)) return Ok("Contrato creado.");
            return StatusCode(500, "No se pudo crear.");
        }

        [HttpPut("ActualizarContrato/{id:int}")]
        public ActionResult Put(int id, [FromBody] ContratoDto dto)
        {
            if (_service.ActualizarContrato(dto)) return Ok("Contrato actualizado.");
            return StatusCode(500, "No se pudo actualizar.");
        }

        [HttpDelete("EliminarContrato/{id:int}")]
        public ActionResult Delete(int id)
        {
            if (_service.EliminarContrato(id)) return Ok("Contrato eliminado.");
            return StatusCode(500, "No se pudo eliminar.");
        }
    }
}
