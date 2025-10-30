using AutoMapper;
using DemoBackend.Dto.Servicio;
using DemoBackend.Models.Servicio;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Servicio;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data; // para SqlDbType

namespace DemoBackend.Services
{
    public class ServicioService : IServicioService
    {
        private readonly IGenericRepositoryEntity<ServicioModels> _repoServicio;
        private readonly IMapper _mapper;

        public ServicioService(
            IGenericRepositoryEntity<ServicioModels> repoServicio,
            IMapper mapper)
        {
            _repoServicio = repoServicio;
            _mapper = mapper;
        }

        #region Servicios

        // CREATE
        public bool CrearServicio(ServicioDto servicio)
        {
            // MAN_CRE_Servicio @NombreServicio,@idTipoServicio,@idEmpresa,@Estado,
            //                  @idServicioPrioridad,@idServiciosCategoria,
            //                  @TiempoEsttimado,@Precio
            const string sql = "MAN_CRE_Servicio " +
                               "@NombreServicio,@idTipoServicio,@idEmpresa,@Estado," +
                               "@idServicioPrioridad,@idServiciosCategoria," +
                               "@TiempoEsttimado,@Precio";

            var pPrecio = new SqlParameter("@Precio", SqlDbType.Int)
            {
                Value = servicio.Precio
            };

            var parametros = new[]
            {
                new SqlParameter("@NombreServicio", (object?)servicio.NombreServicio ?? DBNull.Value),
                new SqlParameter("@idTipoServicio", servicio.IdTipoServicio),
                new SqlParameter("@idEmpresa", servicio.IdEmpresa),
                new SqlParameter("@Estado", servicio.Estado),

                new SqlParameter("@idServicioPrioridad", (object?)servicio.IdServicioPrioridad ?? DBNull.Value),
                new SqlParameter("@idServiciosCategoria", (object?)servicio.IdServiciosCategoria ?? DBNull.Value),

                // En BD la columna es TiempoEsttimado (doble 't')
                new SqlParameter("@TiempoEsttimado", servicio.TiempoEstimadoMinutos),
                pPrecio
            };

            try
            {
                _repoServicio.InsertProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        // UPDATE
        public bool ModificarServicio(ServicioDto servicio)
        {
            // MAN_UPD_Servicio @idServicio,@NombreServicio,@idTipoServicio,@idEmpresa,@Estado,
            //                  @idServicioPrioridad,@idServiciosCategoria,
            //                  @TiempoEsttimado,@Precio
            const string sql = "MAN_UPD_Servicio " +
                               "@idServicio,@NombreServicio,@idTipoServicio,@idEmpresa,@Estado," +
                               "@idServicioPrioridad,@idServiciosCategoria," +
                               "@TiempoEsttimado,@Precio";

            var pPrecio = new SqlParameter("@Precio", SqlDbType.Int)
            {
                Value = servicio.Precio
            };

            var parametros = new[]
            {
                new SqlParameter("@idServicio", servicio.IdServicio),
                new SqlParameter("@NombreServicio", (object?)servicio.NombreServicio ?? DBNull.Value),
                new SqlParameter("@idTipoServicio", servicio.IdTipoServicio),
                new SqlParameter("@idEmpresa", servicio.IdEmpresa),
                new SqlParameter("@Estado", servicio.Estado),

                new SqlParameter("@idServicioPrioridad", (object?)servicio.IdServicioPrioridad ?? DBNull.Value),
                new SqlParameter("@idServiciosCategoria", (object?)servicio.IdServiciosCategoria ?? DBNull.Value),

                new SqlParameter("@TiempoEsttimado", servicio.TiempoEstimadoMinutos),
                pPrecio
            };

            try
            {
                _repoServicio.ExecuteProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        // DELETE (lógico)
        public bool EliminarServicio(ServicioDto servicio)
        {
            const string sql = "MAN_DEL_Servicio @idServicio";

            var parametros = new[]
            {
                new SqlParameter("@idServicio", servicio.IdServicio)
            };

            try
            {
                _repoServicio.ExecuteProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        // LIST ALL
        public List<ServicioDto> GetListaServicio()
        {
            const string sql = "MAN_LIST_Servicio";
            var lista = _repoServicio.GetStoreProcedure(sql);
            return _mapper.Map<List<ServicioDto>>(lista);
        }

        // LIST BY ESTADO (1 activo, 0 inactivo)
        public List<ServicioDto> GetListaServicioEstado(int estado)
        {
            const string sql = "MAN_LIST_Servicio_Estado @Estado";
            var parametros = new[] { new SqlParameter("@Estado", estado) };
            var lista = _repoServicio.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<ServicioDto>>(lista);
        }

        // GET BY ID
        public List<ServicioDto> VerificaServicioPorId(ServicioDto servicio)
        {
            const string sql = "MAN_GET_ServicioById @idServicio";
            var parametros = new[] { new SqlParameter("@idServicio", servicio.IdServicio) };
            var lista = _repoServicio.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<ServicioDto>>(lista);
        }

        // Opcionales

        public List<ServicioDto> GetListaServicioPorCategoria(int? idServiciosCategoria)
        {
            const string sql = "MAN_LIST_Servicio_Categoria @idServiciosCategoria";
            var parametros = new[] {
                new SqlParameter("@idServiciosCategoria", (object?)idServiciosCategoria ?? DBNull.Value)
            };
            var lista = _repoServicio.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<ServicioDto>>(lista);
        }

        public List<ServicioDto> GetListaServicioPorPrioridad(int? idServicioPrioridad)
        {
            const string sql = "MAN_LIST_Servicio_Prioridad @idServicioPrioridad";
            var parametros = new[] {
                new SqlParameter("@idServicioPrioridad", (object?)idServicioPrioridad ?? DBNull.Value)
            };
            var lista = _repoServicio.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<ServicioDto>>(lista);
        }

        #endregion
    }
}
