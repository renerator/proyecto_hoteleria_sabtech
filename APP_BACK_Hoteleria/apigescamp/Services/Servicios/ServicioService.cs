using AutoMapper;
using DemoBackend.Dto.Servicio;
using DemoBackend.Dto.ServicioCategoria;
using DemoBackend.Dto.ServicioEstado;
using DemoBackend.Dto.ServicioPrioridad;
using DemoBackend.Models.Servicio;
using DemoBackend.Models.ServicioCategoria;
using DemoBackend.Models.ServicioPrioridad;
using DemoBackend.Models.ServicioEstado;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Servicio;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data; // para SqlDbType
using System.Linq;


namespace DemoBackend.Services
{
    public class ServicioService : IServicioService
    {
        private readonly IGenericRepositoryEntity<ServicioModels> _repoServicio;        
        private readonly IGenericRepositoryEntity<ServicioKpi> _repoServicioKpi;
        private readonly IGenericRepositoryEntity<ServicioCategoriaModels> _repoCategoriaServicio;
        private readonly IGenericRepositoryEntity<ServicioPrioridadModels> _repoPrioridadServicio;
        private readonly IGenericRepositoryEntity<ServicioEstadoModels> _repoEstadoServicio;
        private readonly IMapper _mapper;

        public ServicioService(
            IGenericRepositoryEntity<ServicioModels> repoServicio,
            IGenericRepositoryEntity<ServicioCategoriaModels> repoCategoriaServicio,
            IGenericRepositoryEntity<ServicioPrioridadModels> repoPrioridadServicio,
            IGenericRepositoryEntity<ServicioEstadoModels> repoEstadoServicio,
             IGenericRepositoryEntity<ServicioKpi> repoServicioKpi,
        IMapper mapper)
        {
            _repoServicio = repoServicio;
            _repoCategoriaServicio = repoCategoriaServicio;
            _repoServicio = repoServicio;
            _repoPrioridadServicio = repoPrioridadServicio;
            _repoEstadoServicio = repoEstadoServicio;
            _repoServicioKpi = repoServicioKpi;
            _mapper = mapper;
        }

        #region Servicios

        // CREATE
        public bool CrearServicio(ServicioDto servicio)
        {
            const string sql = "MAN_CRE_Servicio " +
                               "@NombreServicio,@idTipoServicio,@idEmpresa,@Estado," +
                               "@idServicioPrioridad,@idServiciosCategoria," +
                               "@TiempoEsttimado,@IdServicioEstado,@Precio";

            var parametros = new[]
            {
        new SqlParameter("@NombreServicio", (object)servicio.NombreServicio ?? DBNull.Value),
        new SqlParameter("@idTipoServicio", servicio.IdTipoServicio),
        new SqlParameter("@idEmpresa", servicio.IdEmpresa),
        new SqlParameter("@Estado", servicio.Estado),

        new SqlParameter("@idServicioPrioridad",
            (object?)servicio.IdServicioPrioridad ?? DBNull.Value),

        new SqlParameter("@idServiciosCategoria",
            (object?)servicio.IdServiciosCategoria ?? DBNull.Value),

        // 👇 AHORA SÍ: usamos el estado del servicio, no la categoría
        new SqlParameter("@IdServicioEstado",
            servicio.IdServicioEstado.HasValue && servicio.IdServicioEstado.Value > 0
                ? (object)servicio.IdServicioEstado.Value
                : DBNull.Value),

        new SqlParameter("@TiempoEsttimado", servicio.TiempoEstimadoMinutos),

        new SqlParameter("@Precio", SqlDbType.Int)
        {
            Value = servicio.Precio.HasValue ? servicio.Precio.Value : 0
        }
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
                               "@TiempoEsttimado,@Precio,@IdServicioEstado";

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
                pPrecio,
                 new SqlParameter("@IdServicioEstado", (object?)servicio.IdServicioEstado ?? DBNull.Value)
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
        public List<ServicioEstadoDto> GetListaServicioEstado(int estado)
        {
            const string sql = "MAN_LIST_Servicio_Estado @vigencia";
            var parametros = new[] { new SqlParameter("@vigencia", estado) };
            var lista = _repoEstadoServicio.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<ServicioEstadoDto>>(lista);
        }

        // GET BY ID
        public List<ServicioDto> VerificaServicioPorId(ServicioDto servicio)
        {
            const string sql = "MAN_GET_ServicioById @idServicio, @NombreServicio";

            var parametros = new[]
            {
        new SqlParameter("@idServicio", servicio.IdServicio == 0 ? (object)DBNull.Value : servicio.IdServicio),
        new SqlParameter("@NombreServicio", string.IsNullOrWhiteSpace(servicio.NombreServicio)
                                           ? (object)DBNull.Value
                                           : servicio.NombreServicio)
    };

            var lista = _repoServicio.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<ServicioDto>>(lista);
        }


        // Opcionales

        public List<ServicioCategoriaDto> GetListaServiciosCategoria(int vigencia)
        {
            const string sql = "MAN_LIST_Servicio_Categoria @vigencia";
            var parametros = new[] {
                new SqlParameter("@vigencia", (object?)vigencia ?? DBNull.Value)
            };
            var lista = _repoCategoriaServicio.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<ServicioCategoriaDto>>(lista);
        }

        public List<ServicioPrioridadDto> GetListaServicioPrioridad(int vigencia)
        {
            const string sql = "MAN_LIST_Servicio_Prioridad @vigencia";
            var parametros = new[] {
                new SqlParameter("@vigencia", (object?)vigencia ?? DBNull.Value)
            };
            var lista = _repoPrioridadServicio.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<ServicioPrioridadDto>>(lista);
        }
        public ServicioKpiDto GetKpiServicios()
        {
            const string sql = "MAN_LIST_ServicioKPI";

            // esto devuelve IEnumerable<ServicioKpiModels>
            var resultados = _repoServicioKpi.GetStoreProcedure(sql);
          
            // tomamos el primero "a mano"
            ServicioKpi kpi = null;
            foreach (var r in resultados)
            {
                kpi = r;
                break;              // solo queremos el primero
            }

            if (kpi == null)
                kpi = new ServicioKpi();

            return new ServicioKpiDto
            {
                TotalServicios = kpi.TotalServicios ?? 0,
                ServiciosActivos = kpi.ServiciosActivos ?? 0,
                Categorias = kpi.Categorias ?? 0,
                PromedioMinutos = kpi.PromedioMinutos ?? 0
            };
        }


        #endregion
    }
}
