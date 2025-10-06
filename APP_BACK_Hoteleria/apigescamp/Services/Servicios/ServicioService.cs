using AutoMapper;
using DemoBackend.Dto.Servicio;
using DemoBackend.Models.Servicio;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Servicio;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

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

        #region Servicios (SP: ajusta nombres si en tu BD son distintos)
        // CREATE
        public bool CrearServicio(ServicioDto servicio)
        {
            // Ejemplo de SP: MAN_CRE_Servicio
            const string sql = "MAN_CRE_Servicio @NombreServicio,@idTipoServicio,@idEmpresa,@Estado";

            var parametros = new[]
            {
                new SqlParameter("@NombreServicio", (object?)servicio.NombreServicio ?? DBNull.Value),
                new SqlParameter("@idTipoServicio", servicio.IdTipoServicio),
                new SqlParameter("@idEmpresa", servicio.IdEmpresa),
                new SqlParameter("@Estado", servicio.Estado)
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
            // Ejemplo de SP: MAN_UPD_Servicio
            const string sql = "MAN_UPD_Servicio @idServicio,@NombreServicio,@idTipoServicio,@idEmpresa,@Estado";

            var parametros = new[]
            {
                new SqlParameter("@idServicio", servicio.IdServicio),
                new SqlParameter("@NombreServicio", (object?)servicio.NombreServicio ?? DBNull.Value),
                new SqlParameter("@idTipoServicio", servicio.IdTipoServicio),
                new SqlParameter("@idEmpresa", servicio.IdEmpresa),
                new SqlParameter("@Estado", servicio.Estado)
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

        // DELETE (lógico o físico según tu SP)
        public bool EliminarServicio(ServicioDto servicio)
        {
            // Ejemplo de SP: MAN_DEL_Servicio
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
            // Ejemplo de SP: MAN_LIST_Servicio
            const string sql = "MAN_LIST_Servicio";

            var lista = _repoServicio.GetStoreProcedure(sql);
            return _mapper.Map<List<ServicioDto>>(lista);
        }

        // LIST BY ESTADO (1 activo, 0 inactivo)
        public List<ServicioDto> GetListaServicioEstado(int estado)
        {
            // Ejemplo de SP: MAN_LIST_Servicio_Estado
            const string sql = "MAN_LIST_Servicio_Estado @Estado";

            var parametros = new[]
            {
                new SqlParameter("@Estado", estado)
            };

            var lista = _repoServicio.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<ServicioDto>>(lista);
        }

        // GET BY ID (para verificar existencia)
        public List<ServicioDto> VerificaServicioPorId(ServicioDto servicio)
        {
            // Ejemplo de SP: MAN_GET_ServicioById
            const string sql = "MAN_GET_ServicioById @idServicio";

            var parametros = new[]
            {
                new SqlParameter("@idServicio", servicio.IdServicio)
            };

            var lista = _repoServicio.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<ServicioDto>>(lista);
        }
        #endregion
    }
}
