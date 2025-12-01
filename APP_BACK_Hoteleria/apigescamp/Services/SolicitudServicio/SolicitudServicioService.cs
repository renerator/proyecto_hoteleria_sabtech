using AutoMapper;
using DemoBackend.Dto.Reserva;
using DemoBackend.Dto.SolicitudServicio;
using DemoBackend.Models.SolicitudServicio;
using DemoBackend.RepositoryGes;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DemoBackend.Services.SolicitudServicio
{
    public class SolicitudServicioService : ISolicitudServicioService
    {//cambio 1-12
        private readonly IGenericRepositoryEntity<SolicitudServicioModels> _service;
        private readonly IGenericRepositoryEntity<SolicitudKPIModels> _serviceKPI;
        private readonly IMapper _mapper;

        public SolicitudServicioService(
            IGenericRepositoryEntity<SolicitudServicioModels> repo,
            IGenericRepositoryEntity<SolicitudKPIModels> repoKPI,
            IMapper mapper)
        {
            _service = repo;
            _serviceKPI = repoKPI;
            _mapper = mapper;
        }

        // =================== BÚSQUEDA GENERAL ===================
        public List<SolicitudServicioDto> Buscar(
            int? idSolicitud = null,
            int? idHabitacion = null,
            int? idServicio = null,
            DateTime? desde = null,
            DateTime? hasta = null)
        {
            string sql = "SOL_BUS_SolicitudServicio @idSolicitud,@idHabitacion,@idServicio,@Desde,@Hasta";

            var p = new SqlParameter[]
            {
                new SqlParameter("@idSolicitud",   (object?)idSolicitud   ?? DBNull.Value),
                new SqlParameter("@idHabitacion", (object?)idHabitacion ?? DBNull.Value),
                new SqlParameter("@idServicio",   (object?)idServicio   ?? DBNull.Value),
                new SqlParameter("@Desde",        (object?)desde        ?? DBNull.Value),
                new SqlParameter("@Hasta",        (object?)hasta        ?? DBNull.Value)
            };

            var data = _service.GetStoreProcedure(sql, p)?.ToList()
                       ?? new List<SolicitudServicioModels>();

            return _mapper.Map<List<SolicitudServicioDto>>(data);
        }

        public SolicitudServicioDto? ObtenerPorId(int idSolicitud)
        {
            var list = Buscar(idSolicitud: idSolicitud);
            return list.FirstOrDefault();
        }

        // =================== CREAR ===================
        public bool Crear(SolicitudServicioDto dto)
        {
            if (dto == null) return false;
            if (dto.IdHabitacion <= 0 || dto.IdTipoServicio <= 0) return false;

            // si no viene fecha, dejamos ahora
            var fecha = dto.FechaSolicitud ?? DateTime.Now;

            string sql = "SOL_CRE_SolicitudServicio " +
                         "@idHabitacion,@idServicio,@FechaSolicitud," +
                         "@idPersonalAsignado,@idOrdenTrabajo," +
                         "@idSolicitante,@idTipoServicio,@idEstadoSolicitud," +
                         "@idEmpresa,@Descripcion,@idPrioridad,@idEstado";

            var p = new SqlParameter[]
            {
                new SqlParameter("@idHabitacion",       dto.IdHabitacion),
                new SqlParameter("@idServicio",         dto.IdServicio),
                new SqlParameter("@FechaSolicitud",     fecha),
                new SqlParameter("@idPersonalAsignado", (object?)dto.IdPersonalAsignado ?? DBNull.Value),
                new SqlParameter("@idOrdenTrabajo",     (object?)dto.IdOrdenTrabajo     ?? DBNull.Value),
                new SqlParameter("@idSolicitante",      (object?)dto.IdSolicitante      ?? DBNull.Value),
                new SqlParameter("@idTipoServicio",     (object?)dto.IdTipoServicio     ?? DBNull.Value),
                new SqlParameter("@idEstadoSolicitud",  (object?)dto.IdEstadoSolicitud  ?? DBNull.Value),

                // nuevos campos según la tabla hot_SolicitudServicios
                new SqlParameter("@idEmpresa",          (object?)dto.IdEstadoSolicitud          ?? DBNull.Value),
                new SqlParameter("@Descripcion",        (object?)dto.Descripcion        ?? DBNull.Value),
                new SqlParameter("@idPrioridad",        (object?)dto.idPrioridad        ?? DBNull.Value),
                // si no viene IdEstado, dejamos 1 (activo)
                new SqlParameter("@idEstado",           (object?)dto.idEstado           ?? 1)
            };

            try
            {
                _service.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        // =================== KPI ===================
        public Task<SolicitudKPIDto> ObtenerKpiAsync()
        {
            string sql = "HOT_DASH_SolicitudServicio";
            var parametros = Array.Empty<SqlParameter>();

            var dto = new SolicitudKPIDto();

            try
            {
                var lista = _serviceKPI.GetStoreProcedure(sql, parametros)?.ToList()
                    ?? new List<SolicitudKPIModels>();

                Console.WriteLine($"[ObtenerKpiAsync] Filas devueltas por SP: {lista.Count}");

                var kpiRow = lista.FirstOrDefault();

                if (kpiRow != null)
                {
                    dto.SolicitudPendientesHoy = kpiRow.SolicitudPendientesHoy;
                    dto.SolicitudPendientesSemana = kpiRow.SolicitudPendientesSemana;
                    dto.TiempoPromedioRespuesta = kpiRow.TiempoPromedioRespuesta;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerKpiAsync: {ex.Message}");
            }

            return Task.FromResult(dto);
        }

        // =================== LISTA POR ESTADO (VIGENCIA / ESTADO SOLICITUD) ===================
        public List<SolicitudServicioDto> GetListaSolicitudServicioEstado(
            int idEstado,
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            // idEstado = idEstadoSolicitud (1=Pendiente, 2=Asignada, etc.)
            const string sql = "SOL_BUS_SolicitudServicio_Vigencia @IdEstado,@FechaInicio,@FechaFin";

            var p = new[]
            {
                new SqlParameter("@IdEstado", SqlDbType.Int)
                {
                    Value = idEstado
                },
                new SqlParameter("@FechaInicio", SqlDbType.DateTime)
                {
                    Value = (object?)fechaInicio ?? DBNull.Value,
                    IsNullable = true
                },
                new SqlParameter("@FechaFin", SqlDbType.DateTime)
                {
                    Value = (object?)fechaFin ?? DBNull.Value,
                    IsNullable = true
                }
            };

            var data = _service.GetStoreProcedure(sql, p)?.ToList()
                       ?? new List<SolicitudServicioModels>();

            return _mapper.Map<List<SolicitudServicioDto>>(data);
        }

        // =================== MODIFICAR ===================
        public bool Modificar(SolicitudServicioDto dto)
        {
            if (dto == null || dto.IdSolicitud <= 0) return false;

            string sql = "SOL_UPD_SolicitudServicio " +
                         "@idSolicitud,@idHabitacion,@idServicio,@FechaSolicitud," +
                         "@idPersonalAsignado,@idOrdenTrabajo," +
                         "@idSolicitante,@idTipoServicio,@idEstadoSolicitud," +
                         "@idEmpresa,@Descripcion,@idPrioridad,@idEstado";

            var p = new SqlParameter[]
            {
                new SqlParameter("@idSolicitud",        dto.IdSolicitud),
                new SqlParameter("@idHabitacion",       dto.IdHabitacion),
                new SqlParameter("@idServicio",         dto.IdServicio),
                new SqlParameter("@FechaSolicitud",     (object?)dto.FechaSolicitud     ?? DBNull.Value),
                new SqlParameter("@idPersonalAsignado", (object?)dto.IdPersonalAsignado ?? DBNull.Value),
                new SqlParameter("@idOrdenTrabajo",     (object?)dto.IdOrdenTrabajo     ?? DBNull.Value),
                new SqlParameter("@idSolicitante",      (object?)dto.IdSolicitante      ?? DBNull.Value),
                new SqlParameter("@idTipoServicio",     (object?)dto.IdTipoServicio     ?? DBNull.Value),
                new SqlParameter("@idEstadoSolicitud",  (object?)dto.IdEstadoSolicitud  ?? DBNull.Value),

                // nuevos campos
                new SqlParameter("@idEmpresa",          (object?)dto.idEmpresa          ?? DBNull.Value),
                new SqlParameter("@Descripcion",        (object?)dto.Descripcion        ?? DBNull.Value),
                new SqlParameter("@idPrioridad",        (object?)dto.idPrioridad        ?? DBNull.Value),
                new SqlParameter("@idEstado",           (object?)dto.idEstado           ?? DBNull.Value)
            };

            try
            {
                _service.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        // =================== ELIMINAR ===================
        public bool Eliminar(int idSolicitud)
        {
            if (idSolicitud <= 0) return false;

            string sql = "SOL_DEL_SolicitudServicio @idSolicitud";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idSolicitud", idSolicitud),
            };

            try
            {
                _service.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
