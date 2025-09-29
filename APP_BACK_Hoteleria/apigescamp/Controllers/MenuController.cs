using DemoBackend.Dto.Menu;
using DemoBackend.Services.Menu;
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
    public class MenuController : BaseController
    {
        private readonly IMenuService _grupoService;
        private readonly ILogger _logger;

        public MenuController(IMenuService manService, ILogger<MenuController> logger)
        {
            _logger = logger;
            _grupoService = manService;
        }


        /// <summary>
        /// Servicio que retorna el listado de las Areas 
        /// </summary>
        /// <returns>lista areas</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("GetMenu")]
        public ActionResult<List<MenuDto>> GetListaMenu(int IdUsuario, int IdPerfil)
        {
            _logger.LogInformation($"GetMenu : Inicio proceso lista de Areas");
            try
            {
                var grupos = _grupoService.GetListaMenu(IdUsuario, IdPerfil);
                if (grupos.Count == 0)
                {
                    _logger.LogInformation("GetListaAreas : El proceso de lista de Areas");
                    return new NoContentResult();
                }
                else
                {
                    _logger.LogInformation($"GetListaAreas : El proceso de lista de Areas retorna una lista de {grupos.Count} encontrados.");
                    return Ok(grupos);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"GetListaAreas : El proceso de lista de Areas se ejecuta con error --> {e.Message}");
                _logger.LogTrace($"GetListaAreas : Traza del error --> {e.StackTrace}");
                return StatusCode(500, e.Message);
            }
        }


      




    }
}
