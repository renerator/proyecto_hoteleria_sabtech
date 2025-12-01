using DemoBackend.Dto.Empresa;
using DemoBackend.Services.Empresa;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using System;

using System.Net;

[ApiController]
[Route("api/[controller]")]
public class EmpresaController : ControllerBase
{
    private readonly IEmpresaService _service;
    public EmpresaController(IEmpresaService service) => _service = service;

    // GET /api/Empresas/combo?soloActivas=true&filtro=abc
    // GET /api/Empresas/combo?soloActivas=true&filtro=abc
    [HttpGet("combo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Combo([FromQuery] bool? soloActivas = true, [FromQuery] string? filtro = null)
    {
        var data = _service.Listar(soloActivas, filtro) ?? new List<EmpresaDto>();
        if (data.Count == 0) return NoContent();
        return Ok(data);
    }
    //cambio 1-12
    // Cambia solo la acción Crear; el resto igual a Opción A
    [HttpPost("crear")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Crear([FromBody] EmpresaCrearDto dto)
    {
        if (dto is null) return BadRequest("Se requiere el cuerpo de la solicitud.");
        if (string.IsNullOrWhiteSpace(dto.NombreEmpresaContratista))
        {
            ModelState.AddModelError(nameof(dto.NombreEmpresaContratista), "El nombre de la empresa es obligatorio.");
            return ValidationProblem(ModelState);
        }

        var ok = _service.Crear(dto); // <-- bool
        if (!ok)
            return Problem(title: "No se pudo crear la empresa", statusCode: StatusCodes.Status500InternalServerError);

        // 201 sin Location (no tenemos id). Alternativa: return Ok(true);
        return StatusCode(StatusCodes.Status201Created);
    }

}
