using System.Collections.Generic;
using DemoBackend.Dto.ServiciosPersonal;
using DemoBackend.Services.ServiciosPersonal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoBackend.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class ServiciosPersonalController : ControllerBase
    {
        private readonly IServiciosPersonalService _service;
        //cambio 1-12
        public ServiciosPersonalController(IServiciosPersonalService service)
        {
            _service = service;
        }

        [HttpGet("Kpi")]
        public ActionResult<ServiciosPersonalKpiDto> Kpi()
        {
            return Ok(_service.GetKpi());
        }

        [HttpGet("Solicitudes")]
        public ActionResult<List<ServiciosPersonalDto>> Solicitudes()
        {
            return Ok(_service.GetSolicitudes(null));
        }

        //[HttpGet("Activos")]
        //public ActionResult<List<ServiciosPersonalDto>> Activos()
        //{
        //    return Ok(_service.ListarActivos());
        //}

        //[HttpGet("Proximos")]
        //public ActionResult<List<ServiciosPersonalDto>> Proximos()
        //{
        //    return Ok(_service.ListarProximos());
        //}

        //[HttpPost("Asignar")]
        //public ActionResult Asignar([FromBody] ServiciosPersonalDto dto)
        //{
        //    if (_service.Asignar(dto)) return Ok("Asignado.");
        //    return StatusCode(500, "No se pudo asignar.");
        //}

        //[HttpPost("Iniciar")]
        //public ActionResult Iniciar([FromBody] ServiciosPersonalDto dto)
        //{
        //    if (_service.Iniciar(dto)) return Ok("Iniciado.");
        //    return StatusCode(500, "No se pudo iniciar.");
        //}

        //[HttpPost("Completar")]
        //public ActionResult Completar([FromBody] ServiciosPersonalDto dto)
        //{
        //    if (_service..Completar(dto)) return Ok("Completado.");
        //    return StatusCode(500, "No se pudo completar.");
        //}

        //[HttpPost("Notificar")]
        //public ActionResult Notificar([FromBody] ServiciosPersonalDto dto)
        //{
        //    if (_service.Notificar(dto)) return Ok("Notificado.");
        //    return StatusCode(500, "No se pudo notificar.");
        //}
    }
}
