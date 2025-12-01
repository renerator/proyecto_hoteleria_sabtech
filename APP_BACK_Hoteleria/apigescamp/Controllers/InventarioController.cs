using System.Collections.Generic;
using DemoBackend.Dto.Inventario;
using DemoBackend.Services.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoBackend.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioService _service;
        //cambio 1-12
        public InventarioController(IInventarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<InventarioItemDto>> Get(
            [FromQuery] string? criterio,
            [FromQuery] string? categoria,
            [FromQuery] string? estado,
            [FromQuery] string? habitacion)
        {
            return Ok(_service.GetInventario(criterio, categoria, estado, habitacion));
        }

        [HttpGet("resumen")]
        public ActionResult<InventarioKpiDto> Resumen()
        {
            return Ok(_service.GetKpi());
        }

        [HttpGet("{id}")]
        public ActionResult<InventarioItemDto> GetById(int id)
        {
            var dto = _service.GetItem(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost("CrearInventario/")]
        public ActionResult Post([FromBody] InventarioItemDto dto)
        {
            if (_service.CrearItem(dto)) return Ok("Artículo creado.");
            return StatusCode(500, "No se pudo crear.");
        }

        [HttpPut("ActualizarInventario/{id}")]
        public ActionResult Put(string id, [FromBody] InventarioItemDto dto)
        {
            if (_service.ActualizarItem(dto)) return Ok("Artículo actualizado.");
            return StatusCode(500, "No se pudo actualizar.");
        }

        [HttpDelete("EliminarInventario/{id}")]
        public ActionResult Delete(int id)
        {
            if (_service.EliminarItem(id)) return Ok("Artículo eliminado.");
            return StatusCode(500, "No se pudo eliminar.");
        }

        [HttpGet("{id}/movimientos")]
        public ActionResult<List<InventarioMovimientoPostDto>> Movimientos(int id)
        {
            return Ok(_service.GetMovimientos(id));
        }
    }
}
