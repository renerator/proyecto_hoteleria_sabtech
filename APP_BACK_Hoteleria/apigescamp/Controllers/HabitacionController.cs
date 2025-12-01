using DemoBackend.Dto.BitacoraHabitacion;
using DemoBackend.Dto.Habitacion;
using DemoBackend.Dto.TipoHabitacion;
using DemoBackend.Services;
using DemoBackend.Services.Habitacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DemoBackend.Controllers
{

    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class HabitacionController : BaseController
    {
        private readonly IHabitacionService _grupoService;
        private readonly ILogger _logger;

        public HabitacionController(IHabitacionService manService, ILogger<HabitacionController> logger)
        {
            _logger = logger;
            _grupoService = manService;
        }
        //cambio 1-12

        /// <summary>
        /// Servicio que retorna el listado de las Habitacion 
        /// </summary>
        /// <returns>lista Habitacion</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("HabitacionesDisponibles")]
        public ActionResult<List<HabitacionDto>> HabitacionDisponibles(int vigencia)
        {
            _logger.LogInformation($"GetListaHabitacion : Inicio proceso lista de Habitacion");
            try
            {
                var grupos = _grupoService.GetListaHabitacionEstado(vigencia);
                if (grupos.Count == 0)
                {
                    _logger.LogInformation("GetListaHabitacion : El proceso de lista de Habitacion");
                    return new NoContentResult();
                }
                else
                {
                    _logger.LogInformation($"GetListaHabitacion : El proceso de lista de Habitacion retorna una lista de {grupos.Count} encontrados.");
                    return Ok(grupos);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"GetListaHabitacion : El proceso de lista de Habitacion se ejecuta con error --> {e.Message}");
                _logger.LogTrace($"GetListaHabitacion : Traza del error --> {e.StackTrace}");
                return StatusCode(500, e.Message);
            }
        }


        /// <summary>
        /// Servicio para Crear Habitacion.
        /// </summary>      
        /// <returns>true o false</returns>
        /// <response code="200">Inserción existosa</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpPost("SolicitaHabitacion")]
        public ActionResult SolicitaHabitacion(HabitacionDto HabitacionDto)
        {

            try
            {
                var grupoOK = false;

                //if (string.IsNullOrEmpty(HabitacionModels.NombreHabitacion) || string.IsNullOrEmpty(HabitacionModels.AcronimoArea))
                if (string.IsNullOrEmpty(HabitacionDto.NombreHabitacion))
                {
                    _logger.LogInformation($"PostCreaHabitacion: Vacio, no se graban datos, retorna OK.");
                    return Ok("Status 200: Error de campos vacios");
                }
                else
                {
                    var resu = _grupoService.VerificaHabitacionPorNombre(HabitacionDto);
                    if (resu.Count > 0)
                    {
                        return Ok("Status 200: No se puede crear el area, ya existe el area: " + HabitacionDto.NombreHabitacion);
                    }
                    else
                    {
                        grupoOK = _grupoService.CrearHabitacion(HabitacionDto);
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
                _logger.LogError($"PostCreaHabitacion: Error en grabacion de Habitacion --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }


        /// <summary>
        /// Servicio para Crear Habitacion.
        /// </summary>      
        /// <returns>true o false</returns>
        /// <response code="200">Inserción existosa</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response> 
        /// <response code="500">Error interno</response>
        [HttpPost("ConfirmarHabitacion")]
        public ActionResult ConfirmarHabitacion(HabitacionDto HabitacionModels)
        {

            try
            {
                var grupoOK = false;

                if (string.IsNullOrEmpty(HabitacionModels.NombreHabitacion))
                {
                    _logger.LogInformation($"PostCreaHabitacion: Vacio, no se graban datos, retorna OK.");
                    return Ok("Status 200: Error de campos vacios");
                }
                else
                {
                    var resu = _grupoService.VerificaHabitacionPorNombre(HabitacionModels);
                    if (resu.Count > 0)
                    {
                        return Ok("Status 200: No se puede crear el area, ya existe el area: " + HabitacionModels.NombreHabitacion);
                    }
                    else
                    {
                        grupoOK = _grupoService.CrearHabitacion(HabitacionModels);
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
                _logger.LogError($"PostCreaHabitacion: Error en grabacion de Habitacion --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Servicio para Modificar Habitacion.
        /// </summary>     
        /// <returns>true o false</returns>
        /// <response code="200">Modificacioón existosa</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpPut("ModificaHabitacion")]
        public ActionResult ModificaHabitacion(HabitacionDto HabitacionModels)
        {

            try
            {
                var grupoOK = false;

                if (string.IsNullOrEmpty(HabitacionModels.NombreHabitacion))
                {
                    _logger.LogInformation($"PutModificaHabitacion: Vacio, no se graban datos, retorna OK.");
                    return Ok("Status 200: Error de campos vacios");
                }
                else
                {
                    var resu = _grupoService.VerificaHabitacionPorId(HabitacionModels);
                    if (resu.Count > 0)
                    {
                        grupoOK = _grupoService.ModificarHabitacion(HabitacionModels);
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
                        return Ok("Status 200: No se puede modificar el area, ya que no existe el area: " + HabitacionModels.IdArea);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"PutModificaHabitacion: Error en modificación de Habitacion --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Servicio para Eliminación Habitacion.
        /// </summary>

        /// <param name="idHabitacion">Id de la Habitación</param>
        /// <returns>true o false</returns>
        /// <response code="200">Modificacioón existosa</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpDelete("EliminaHabitacion")]
        public ActionResult EliminaHabitacion(int idHabitacion)
        {
            HabitacionDto HabitacionDto;

            try
            {
                var grupoOK = false;

                if (idHabitacion == 0)
                {
                    _logger.LogInformation($"DelEliminaHabitacion: Vacio, no se graban datos, retorna OK.");
                    return Ok("Status 200: Error de campos vacios");
                }
                else
                {
                    HabitacionDto = new HabitacionDto() { IdHabitacion = idHabitacion };
                    var resu = _grupoService.VerificaHabitacionPorId(HabitacionDto);
                    if (resu.Count > 0)
                    {
                        grupoOK = _grupoService.EliminarHabitacion(HabitacionDto);
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
                        return Ok("Status 200: No se puede eliminar el area, ya que no existe el area: " + idHabitacion);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"DelEliminaHabitacion: Error en eliminación de Habitacion --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }

        // -------- DASHBOARD --------
        // Devuelve KPIs del dashboard (nuevas, servicios, checkin, checkout, pendientes, confirmadas, rechazadas, realizadas)
        [HttpGet("dashboardHabitacion")]
        public IActionResult Dashboard()
        {
            var data = _grupoService.ObtenerDashboardHabitacion(); // sin fechas

            // Evitar caché para “tiempo real”
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return Ok(data);


        }
        /// <summary>
        /// Lista los tipos de habitación (SP: HOT_TIPO_HAB_LISTAR).
        /// </summary>
        [HttpGet("listartiposhabitaciones")]
        // [Authorize] // <-- descomenta si tu API está protegida por JWT
        [ProducesResponseType(typeof(List<TipoHabitacionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<TipoHabitacionDto>> Listar()
        {
            try
            {
                var lista = _grupoService.GetListaTipoHabitacion();
                if (lista == null || lista.Count == 0)
                    return NoContent();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[TipoHabitacion/Listar] Error al listar tipos de habitación.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno al listar tipos de habitación.");
            }
        }

        [HttpPost("CrearBitacoraHabitacion")]
        public ActionResult CrearBitacoraHabitacion([FromBody] BitacoraHabitacionDto dto)
        {
            _logger.LogInformation("PostCrearBitacoraHabitacion: inicio.");
            try
            {
                if (dto == null) return BadRequest("Datos vacíos.");
                if (dto.IdHabitacion <= 0) return Ok("Status 200: Error de validación (IdHabitacion requerido).");

                var ok = _grupoService.CrearBitacoraHabitacion(dto);
                return Ok(ok ? "Bitácora creada correctamente." : "No se pudo crear la bitácora.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "PostCrearBitacoraHabitacion: error.");
                return StatusCode(500, e.Message);
            }
        }
    }
}
