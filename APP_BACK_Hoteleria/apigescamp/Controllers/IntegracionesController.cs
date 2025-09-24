using DemoBackend.Dto.Mantenedores;
using DemoBackend.Services.Mantenedores;
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
    public class IntegracionesController : BaseController
    {
        private readonly IMantenedoresService _grupoService;
        private readonly ILogger _logger;

        public IntegracionesController(IMantenedoresService manService, ILogger<IntegracionesController> logger)
        {
            _logger = logger;
            _grupoService = manService;
        }


        /// <summary>
        /// Servicio que retorna el listado de usuarios SGCAS
        /// </summary>
        /// <returns>lista Usuarios</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("GetUsuariosSGCAS")]
        public ActionResult<List<AreasDto>> GetUsuariosSGCAS(int vigencia)
        {
            _logger.LogInformation($"GetListaAreas : Inicio proceso lista de Areas");
            try
            {
                var grupos = _grupoService.GetListaAreasEstado(vigencia);
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

        /// <summary>
        /// Servicio que retorna el listado de las Areas 
        /// </summary>
        /// <returns>lista areas</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("GetCasinoAlimentacion")]
        public ActionResult<List<AreasDto>> GetCasinoAlimentacion(int vigencia)
        {
            _logger.LogInformation($"GetListaAreas : Inicio proceso lista de Areas");
            try
            {
                var grupos = _grupoService.GetListaAreasEstado(vigencia);
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

        /// <summary>
        /// Servicio que retorna el listado de las Areas 
        /// </summary>
        /// <returns>lista areas</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("GetChapasElectronicas")]
        public ActionResult<List<AreasDto>> GetChapasElectronicas(int vigencia)
        {
            _logger.LogInformation($"GetListaAreas : Inicio proceso lista de Areas");
            try
            {
                var grupos = _grupoService.GetListaAreasEstado(vigencia);
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
        /// <summary>
        /// Servicio que retorna el listado de las Areas 
        /// </summary>
        /// <returns>lista areas</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("GetElipse")]
        public ActionResult<List<AreasDto>> GetElipse(int vigencia)
        {
            _logger.LogInformation($"GetListaAreas : Inicio proceso lista de Areas");
            try
            {
                var grupos = _grupoService.GetListaAreasEstado(vigencia);
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
        /// <summary>
        /// Servicio que retorna el listado de las Areas 
        /// </summary>
        /// <returns>lista areas</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("GetSAP")]
        public ActionResult<List<AreasDto>> GetSAP(int vigencia)
        {
            _logger.LogInformation($"GetListaAreas : Inicio proceso lista de Areas");
            try
            {
                var grupos = _grupoService.GetListaAreasEstado(vigencia);
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
        /// <summary>
        /// Servicio que retorna el listado de las Areas 
        /// </summary>
        /// <returns>lista areas</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("GetLavanderia")]
        public ActionResult<List<AreasDto>> GetLavanderia(int vigencia)
        {
            _logger.LogInformation($"GetListaAreas : Inicio proceso lista de Areas");
            try
            {
                var grupos = _grupoService.GetListaAreasEstado(vigencia);
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
