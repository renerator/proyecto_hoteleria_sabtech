using DemoBackend.Dto.BitacoraReserva;
using DemoBackend.Dto.EstadoReserva;
using DemoBackend.Dto.Mantenedores;
using DemoBackend.Dto.Check;
using DemoBackend.Models.Check;
using DemoBackend.Services;
using DemoBackend.Services.Check;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoBackend.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class CheckController : BaseController
    {
        private readonly ICheckinCheckoutService   _checkService;
        private readonly ILogger _logger;
        //cambio 1-12
        public CheckController(ICheckinCheckoutService checkService, ILogger<CheckController> logger)
        {
            _logger = logger;
            _checkService = checkService;
        }

        /// <summary>
        /// Servicio que retorna el listado de Reservas según estado
        /// </summary>
        /// <returns>lista Reservas</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("ReservasCheck")]
        public ActionResult<List<CheckDTO>> ReservasDisponiblesCheck(int idEstadoReserva, DateTime? fechaDesde)
        {
            _logger.LogInformation("GetListaReservas : Inicio proceso lista de Reservas");
            try
            {
                var reservas = _checkService.GetListar(idEstadoReserva, fechaDesde);
                if (reservas.Count == 0)
                {
                    _logger.LogInformation("GetListaReservas : No se encontraron reservas.");
                    return new NoContentResult();
                }
                else
                {
                    _logger.LogInformation($"GetListaReservas : Se encontraron {reservas.Count} reservas.");
                    return Ok(reservas);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"GetListaReservas : Error --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }

        
        

       
       
      

        // -------- DASHBOARD --------
        // Devuelve KPIs del dashboard (nuevas, servicios, checkin, checkout, pendientes, confirmadas, rechazadas, realizadas)
        [HttpGet("ResumenCheckKPI")]
        public IActionResult Dashboard()
        {
            var data = _checkService.ObtenerDashboard();

            // Evitar caché para “tiempo real”
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return Ok(data);
        }
       


    }
}

