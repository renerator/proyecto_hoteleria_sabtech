using AutoMapper;
using DemoBackend.Dto.SolicitudServicio;
using DemoBackend.Models.SolicitudServicio;
using DemoBackend.RepositoryGes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoBackend.Services.SolicitudServicio
{
    public class SolicitudServicioService : ISolicitudServicioService
    {
        private readonly IGenericRepositoryEntity<SolicitudServicioModels> _service;
        private readonly IMapper _mapper;

        public SolicitudServicioService(IGenericRepositoryEntity<SolicitudServicioModels> repo, IMapper mapper)
        {
            _service = repo;
            _mapper = mapper;
        }

        public List<SolicitudServicioDto> Buscar(int? idSolicitud = null, int? idHabitacion = null, int? idServicio = null, DateTime? desde = null, DateTime? hasta = null)
        {
            string sql = "SOL_BUS_SolicitudServicio @idSolicitud,@idHabitacion,@idServicio,@Desde,@Hasta";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idSolicitud", (object?)idSolicitud ?? DBNull.Value),
                new SqlParameter("@idHabitacion", (object?)idHabitacion ?? DBNull.Value),
                new SqlParameter("@idServicio", (object?)idServicio ?? DBNull.Value),
                new SqlParameter("@Desde", (object?)desde ?? DBNull.Value),
                new SqlParameter("@Hasta", (object?)hasta ?? DBNull.Value)
            };

            var data = _service.GetStoreProcedure(sql, p)?.ToList() ?? new List<SolicitudServicioModels>();
            return _mapper.Map<List<SolicitudServicioDto>>(data);
        }

        public SolicitudServicioDto? ObtenerPorId(int idSolicitud)
        {
            var list = Buscar(idSolicitud: idSolicitud);
            return list.FirstOrDefault();
        }

        public bool Crear(SolicitudServicioDto dto)
        {
            if (dto == null) return false;
            if (dto.IdHabitacion <= 0 || dto.IdServicio <= 0) return false;

            string sql = "SOL_CRE_SolicitudServicio @idHabitacion,@idServicio,@FechaSolicitud,@HoraSolicitud,@AtendidoPor,@idOrdenTrabajo,@idTrabajador";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idHabitacion", dto.IdHabitacion),
                new SqlParameter("@idServicio", dto.IdServicio),
                new SqlParameter("@FechaSolicitud", (object?)dto.FechaSolicitud ?? DBNull.Value),
                new SqlParameter("@HoraSolicitud", (object?)dto.HoraSolicitud ?? DBNull.Value),
                new SqlParameter("@AtendidoPor", (object?)dto.AtendidoPor ?? DBNull.Value),
                new SqlParameter("@idOrdenTrabajo", (object?)dto.IdOrdenTrabajo ?? DBNull.Value),
                new SqlParameter("@idTrabajador", (object?)dto.IdTrabajador ?? DBNull.Value)
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

        // SolicitudServicioService
        public List<SolicitudServicioDto> GetListaSolicitudServicioEstado(int vigencia)
        {
           
            var sql = "SOL_BUS_SolicitudServicio_Vigencia @Vigencia";
            var p = new[] { new SqlParameter("@Vigencia", vigencia) };
            var data = _service.GetStoreProcedure(sql, p)?.ToList() ?? new List<SolicitudServicioModels>();
            return _mapper.Map<List<SolicitudServicioDto>>(data);
        }


        public bool Modificar(SolicitudServicioDto dto)
        {
            if (dto == null || dto.IdSolicitud <= 0) return false;

            string sql = "SOL_UPD_SolicitudServicio @idSolicitud,@idHabitacion,@idServicio,@FechaSolicitud,@HoraSolicitud,@AtendidoPor,@idOrdenTrabajo,@idTrabajador";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idSolicitud", dto.IdSolicitud),
                new SqlParameter("@idHabitacion", dto.IdHabitacion),
                new SqlParameter("@idServicio", dto.IdServicio),
                new SqlParameter("@FechaSolicitud", (object?)dto.FechaSolicitud ?? DBNull.Value),
                new SqlParameter("@HoraSolicitud", (object?)dto.HoraSolicitud ?? DBNull.Value),
                new SqlParameter("@AtendidoPor", (object?)dto.AtendidoPor ?? DBNull.Value),
                new SqlParameter("@idOrdenTrabajo", (object?)dto.IdOrdenTrabajo ?? DBNull.Value),
                new SqlParameter("@idTrabajador", (object?)dto.IdTrabajador ?? DBNull.Value)
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