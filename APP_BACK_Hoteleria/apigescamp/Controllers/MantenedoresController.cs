using DemoBackend.Dto.Mantenedores;
using DemoBackend.Services.Mantenedores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DemoBackend.Controllers
{
    public class MantenedoresController : BaseController
    {
        private readonly IMantenedoresService _grupoService;
        private readonly ILogger _logger;

        public MantenedoresController(IMantenedoresService manService, ILogger<MantenedoresController> logger)
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
        /// <response code="500">Error interno</response>
        [HttpGet("GetListaAreas")]
        public ActionResult<List<AreasDto>> GetListaAreas(int vigencia)
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
                    _logger.LogInformation($"GetListaAreas : El proceso de lista de Areas retorna una lista de { grupos.Count } encontrados.");
                    return Ok(grupos);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"GetListaAreas : El proceso de lista de Areas se ejecuta con error --> { e.Message }");
                _logger.LogTrace($"GetListaAreas : Traza del error --> { e.StackTrace }");
                return StatusCode(500, e.Message);
            }
        }


        /// <summary>
        /// Servicio para Crear Areas.
        /// </summary>      
        /// <returns>true o false</returns>
        /// <response code="200">Inserción existosa</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno</response>
        [HttpPost("PostCrearAreas")]
        public ActionResult PostCreaAreas(AreasDto areasModels)
        {

            try
            {
                var grupoOK = false;

                if (string.IsNullOrEmpty(areasModels.NombreArea) )
                {
                    _logger.LogInformation($"PostCreaAreas: Vacio, no se graban datos, retorna OK.");
                    return Ok("Status 200: Error de campos vacios");
                }
                else
                {
                    var resu = _grupoService.VerificaArea(areasModels);
                    if (resu.Count > 0)
                    {
                        return Ok("Status 200: No se puede crear el area, ya existe el area: " + areasModels.NombreArea);
                    }
                    else
                    {
                        grupoOK = _grupoService.CrearAreas(areasModels);
                        if (grupoOK)
                        {
                            return Ok(grupoOK + " OK, Datos insertados");
                        }
                        else
                        {
                            return Ok(grupoOK + " Datos no insertados");
                        }
                       
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"PostCreaAreas: Error en grabacion de Areas --> { e.Message }");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }


        /// <summary>
        /// Servicio para Modificar Areas.
        /// </summary>     
        /// <returns>true o false</returns>
        /// <response code="200">Modificacioón existosa</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno</response>
        [HttpPut("PutModificaAreas")]
        public ActionResult PutModificaAreas(AreasDto areasModels)
        {

            try
            {
                var grupoOK = false;

                if (string.IsNullOrEmpty(areasModels.NombreArea) || areasModels.IdArea == 0)
                {
                    _logger.LogInformation($"PutModificaAreas: Vacio, no se graban datos, retorna OK.");
                    return Ok("Status 200: Error de campos vacios");
                }
                else
                {
                    var resu = _grupoService.VerificaAreaId(areasModels);
                    if (resu.Count > 0)
                    {
                        grupoOK = _grupoService.ModificarAreas(areasModels);
                        if (grupoOK)
                        {
                            return Ok(grupoOK + " OK, Datos modificados");
                        }
                        else
                        {
                            return Ok(grupoOK + " Datos no modificados");
                        }
                                             
                       
                    }
                    else
                    {
                        return Ok("Status 200: No se puede modificar el area, ya que no existe el area: " + areasModels.IdArea);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"PutModificaAreas: Error en modificación de Areas --> { e.Message }");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Servicio para Eliminación Areas.
        /// </summary>
      
        /// <param name="idArea">Id del Area</param>
        /// <returns>true o false</returns>
        /// <response code="200">Modificacioón existosa</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno</response>
        [HttpDelete("DelEliminaAreas")]
        public ActionResult DelEliminaAreas (int idArea)
        {
            AreasDto areasDto;

            try
            {
                var grupoOK = false;

                if ( idArea == 0)
                {
                    _logger.LogInformation($"DelEliminaAreas: Vacio, no se graban datos, retorna OK.");
                    return Ok("Status 200: Error de campos vacios");
                }
                else
                {
                    areasDto = new AreasDto() { IdArea = idArea };
                    var resu = _grupoService.VerificaAreaId(areasDto);
                    if (resu.Count > 0)
                    {
                        grupoOK = _grupoService.EliminarAreas(areasDto);
                        if (grupoOK)
                        {
                            return Ok(grupoOK + " OK, Datos eliminados");
                        }
                        else
                        {
                            return Ok(grupoOK + " Datos no eliminados");
                        }
                      

                    }
                    else
                    {
                        return Ok("Status 200: No se puede eliminar el area, ya que no existe el area: " + idArea);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"DelEliminaAreas: Error en eliminación de Areas --> { e.Message }");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }


       

    }
}
