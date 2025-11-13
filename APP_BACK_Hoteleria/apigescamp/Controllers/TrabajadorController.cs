using DemoBackend.Dto.Insumos;
using DemoBackend.Dto.Trabajador;
using DemoBackend.Services.Trabajador;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DemoBackend.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class TrabajadorController : BaseController
    {
        private readonly ITrabajadorService _trabajadorService;
        private readonly ILogger _logger;

        public TrabajadorController(ITrabajadorService trabajadorService, ILogger<TrabajadorController> logger)
        {
            _logger = logger;
            _trabajadorService = trabajadorService;
        }

        /// <summary>
        /// Servicio que retorna el listado de Trabajadores por vigencia.
        /// </summary>
        /// <returns>Lista de Trabajador</returns>
        /// <response code="204">No encuentra datos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpGet("ListarTrabajadores")]
        public ActionResult<List<TrabajadorDto>> TrabajadoresDisponibles(int IdEmpresa)
        {
            _logger.LogInformation("GetListaTrabajador : Inicio proceso lista de Trabajador");
            try
            {
                var lista = _trabajadorService.GetListaTrabajadorEstado(IdEmpresa);
                if (lista.Count == 0)
                {
                    _logger.LogInformation("GetListaTrabajador : Lista vacía");
                    return new NoContentResult();
                }

                _logger.LogInformation($"GetListaTrabajador : Retorna {lista.Count} registro(s).");
                return Ok(lista);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetListaTrabajador : Error --> {e.Message}");
                _logger.LogTrace($"GetListaTrabajador : Traza --> {e.StackTrace}");
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Servicio para crear Usuario.
        /// </summary>
        /// <response code="200">Inserción exitosa</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        /// 

        [HttpGet("BuscarTrabajadorPorRut")]
        public ActionResult<TrabajadorDto> BuscarTrabajadorRut([FromQuery] string? rut)
        {
            _logger.LogInformation("BuscarTrabajadorRut : inicio. Rut={rut}", rut);

            try
            {
                if (string.IsNullOrWhiteSpace(rut))
                    return BadRequest("Debe indicar el RUT.");

                var dto = _trabajadorService.GetTrabajadorRut(rut);
                if (dto == null)
                {
                    _logger.LogInformation("BuscarTrabajadorRut : no encontrado.");
                    // Puedes usar NotFound() si lo prefieres
                    return new NoContentResult();
                }

                _logger.LogInformation("BuscarTrabajadorRut : encontrado idTrabajador={Id}", dto.IdUsuario);
                return Ok(dto);
            }
            catch (SqlException sqlex)
            {
                _logger.LogError(sqlex, "BuscarTrabajadorRut : error SQL.");
                return StatusCode(500, "Error de base de datos al buscar el trabajador.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "BuscarTrabajadorRut : error.");
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("CrearTrabajador")]
        public ActionResult CrearTrabajador(TrabajadorDto trabajadorDto)
        {
            try
            {
                if (string.IsNullOrEmpty(trabajadorDto.NombresTrabajador))
                {
                    _logger.LogInformation("PostCreaTrabajador: Campos vacíos.");
                    return Ok("Status 200: Error de campos vacíos");
                }

                var existe = _trabajadorService.VerificaTrabajadorPorNombre(trabajadorDto);
                if (existe.Count > 0)
                {
                    return Ok("Status 200: No se puede crear el trabajador, ya existe: " + trabajadorDto.NombresTrabajador);
                }

                var ok = _trabajadorService.CrearTrabajador(trabajadorDto);
                return Ok(ok ? "True OK, Datos insertados" : "False Datos no insertados");
            }
            catch (Exception e)
            {
                _logger.LogError($"PostCreaTrabajador: Error --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }


        /// <summary>
        /// Servicio para modificar Usuario.
        /// </summary>
        /// <response code="200">Modificación exitosa</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpPut("ModificaTrabajador")]
        public ActionResult ModificaTrabajador(TrabajadorDto trabajadorDto)
        {
            try
            {
                if (string.IsNullOrEmpty(trabajadorDto.NombresTrabajador) || trabajadorDto.IdUsuario == 0 || trabajadorDto.IdEmpresaContratista==0)
                {
                    _logger.LogInformation("PutModificaTrabajador: Campos vacíos.");
                    return Ok("Status 200: Error de campos vacíos");
                }

                var existe = _trabajadorService.VerificaTrabajadorPorId(trabajadorDto);
                if (existe.Count == 0)
                {
                    return Ok("Status 200: No se puede modificar el trabajador, no existe Id: " + trabajadorDto.IdUsuario);
                }

                var ok = _trabajadorService.ModificarTrabajador(trabajadorDto);
                return Ok(ok ? "True OK, Datos modificados" : "False Datos no modificados");
            }
            catch (Exception e)
            {
                _logger.LogError($"PutModificaTrabajador: Error --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Servicio para eliminación de Trabajador.
        /// </summary>
        /// <param name="idUsuario">Id del Trabajador</param>
        /// <response code="200">Eliminación exitosa</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado</response>
        /// <response code="500">Error interno</response>
        [HttpDelete("EliminaTrabajador")]
        public ActionResult EliminaTrabajador(int idUsuario)
        {
            try
            {
                if (idUsuario == 0)
                {
                    _logger.LogInformation("DelEliminaTrabajador: IdTrabajador no puede estar vacío.");
                    return Ok("Status 200: Error de campos vacíos");
                }

                var dto = new TrabajadorDto { IdUsuario = idUsuario };
                var existe = _trabajadorService.VerificaTrabajadorPorId(dto);
                if (existe.Count == 0)
                {
                    return Ok("Status 200: No se puede eliminar, no existe Id: " + idUsuario);
                }

                var ok = _trabajadorService.EliminarTrabajador(dto);
                return Ok(ok ? "True OK, Datos eliminados" : "False Datos no eliminados");
            }
            catch (Exception e)
            {
                _logger.LogError($"DelEliminaTrabajador: Error --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }
    }
}
