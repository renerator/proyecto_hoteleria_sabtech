using AutoMapper;
using DemoBackend.Dto.OrdenTrabajo;
using DemoBackend.Models.OrdenTrabajo;
using DemoBackend.RepositoryGes;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoBackend.Services.OrdenTrabajo
{
    public class OrdenTrabajoService : IOrdenTrabajoService
    {
        private readonly IGenericRepositoryEntity<OrdenTrabajoModels> _service;
        private readonly IMapper _mapper;
        //cambio 1-12
        public OrdenTrabajoService(IGenericRepositoryEntity<OrdenTrabajoModels> repo, IMapper mapper)
        {
            _service = repo;
            _mapper  = mapper;
        }

        public List<OrdenTrabajoDto> Buscar(
            int? idOrdenTrabajo = null,
            int? idHabitacion   = null,
            string? numeroOT    = null,
            DateTime? desde     = null,
            DateTime? hasta     = null)
        {
            const string sql = "HOT_OT_BUS_OrdenTrabajo @idOrdenTrabajo,@idHabitacion,@NumeroOT,@Desde,@Hasta";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idOrdenTrabajo", (object?)idOrdenTrabajo ?? DBNull.Value),
                new SqlParameter("@idHabitacion",   (object?)idHabitacion   ?? DBNull.Value),
                new SqlParameter("@NumeroOT",       (object?)numeroOT       ?? DBNull.Value),
                new SqlParameter("@Desde",          (object?)desde          ?? DBNull.Value),
                new SqlParameter("@Hasta",          (object?)hasta          ?? DBNull.Value)
            };

            var data = _service.GetStoreProcedure(sql, p)?.ToList() ?? new List<OrdenTrabajoModels>();
            return _mapper.Map<List<OrdenTrabajoDto>>(data);
        }

        public OrdenTrabajoDto? ObtenerPorId(int idOrdenTrabajo)
        {
            var list = Buscar(idOrdenTrabajo: idOrdenTrabajo);
            return list.FirstOrDefault();
        }

        public bool Crear(OrdenTrabajoDto dto)
        {
            if (dto == null) return false;
            if (dto.IdHabitacion <= 0) return false;
            if (string.IsNullOrWhiteSpace(dto.NumeroOT)) return false;

            const string sql = "HOT_OT_CRE_OrdenTrabajo @NumeroOT,@FechaIngresoOT,@FechaCierreOT,@idHabitacion,@Estado";
            var p = new SqlParameter[]
            {
                new SqlParameter("@NumeroOT",       (object?)dto.NumeroOT       ?? DBNull.Value),
                new SqlParameter("@FechaIngresoOT", (object?)dto.FechaIngresoOT ?? DBNull.Value),
                new SqlParameter("@FechaCierreOT",  (object?)dto.FechaCierreOT  ?? DBNull.Value),
                new SqlParameter("@idHabitacion",   dto.IdHabitacion),
                new SqlParameter("@Estado",         (object?)dto.Estado         ?? DBNull.Value)
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

        public bool Modificar(OrdenTrabajoDto dto)
        {
            if (dto == null || dto.IdOrdenTrabajo <= 0) return false;

            const string sql = "HOT_OT_UPD_OrdenTrabajo @idOrdenTrabajo,@NumeroOT,@FechaIngresoOT,@FechaCierreOT,@idHabitacion,@Estado";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idOrdenTrabajo", dto.IdOrdenTrabajo),
                new SqlParameter("@NumeroOT",       (object?)dto.NumeroOT       ?? DBNull.Value),
                new SqlParameter("@FechaIngresoOT", (object?)dto.FechaIngresoOT ?? DBNull.Value),
                new SqlParameter("@FechaCierreOT",  (object?)dto.FechaCierreOT  ?? DBNull.Value),
                new SqlParameter("@idHabitacion",   dto.IdHabitacion),
                new SqlParameter("@Estado",         (object?)dto.Estado         ?? DBNull.Value)
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

        public bool Eliminar(int idOrdenTrabajo)
        {
            if (idOrdenTrabajo <= 0) return false;

            const string sql = "HOT_OT_DEL_OrdenTrabajo @idOrdenTrabajo";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idOrdenTrabajo", idOrdenTrabajo)
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

        public List<OrdenTrabajoDto> GetListaOrdenTrabajoEstado(int vigencia)
        {
            const string sql = "HOT_OT_BUS_OrdenTrabajo_Vigencia @Vigencia";
            var p = new[] { new SqlParameter("@Vigencia", vigencia) };
            var data = _service.GetStoreProcedure(sql, p)?.ToList() ?? new List<OrdenTrabajoModels>();
            return _mapper.Map<List<OrdenTrabajoDto>>(data);
        }
    }
}
