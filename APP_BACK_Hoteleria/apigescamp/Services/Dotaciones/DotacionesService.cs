using AutoMapper;
using DemoBackend.Dto.Dotaciones;
using DemoBackend.Models.Dotaciones;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Dotaciones;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DemoBackend.Services
{
    public class DotacionesService : IDotacionesService
    {
        private readonly IGenericRepositoryEntity<DotacionesModels> _listaDotaciones;
        private readonly IMapper _mapper;
        //cambio 1-12
        public DotacionesService(
            IGenericRepositoryEntity<DotacionesModels> listaDotaciones,
            IMapper mapper)
        {
            _listaDotaciones = listaDotaciones;
            _mapper = mapper;
        }

        public List<DotacionDto> GetDotaciones(string? filtro)
        {
            const string sql = "HOT_DOT_LISTAR @Filtro";
            var p = new SqlParameter[] { new SqlParameter("@Filtro", (object?)filtro ?? DBNull.Value) };

            var lista = _listaDotaciones.GetStoreProcedure(sql, p);
            return _mapper.Map<List<DotacionDto>>(lista);
        }

        public DotacionDto? GetDotacion(int id)
        {
            const string sql = "HOT_DOT_OBTENER @IdDotacion";
            var p = new SqlParameter[] { new SqlParameter("@IdDotacion", id) };

            var lista = _listaDotaciones.GetStoreProcedure(sql, p);
            var ent = lista.FirstOrDefault();
            return ent == null ? null : _mapper.Map<DotacionDto>(ent);
        }

        public bool CrearDotacion(DotacionDto dto)
        {
            const string sql = "HOT_DOT_CREAR @IdEmpresa,@Empresa,@Estado,@Nombre,@Apellido,@Rut,@Cargo,@Area,@Turno,@HabitacionAsignada,@FechaIngreso,@FechaSalida";
            var p = new SqlParameter[]
            {
                new SqlParameter("@IdEmpresa", (object?)dto.IdEmpresa ?? DBNull.Value),
                new SqlParameter("@Empresa", (object?)dto.Empresa ?? DBNull.Value),
                new SqlParameter("@Estado", dto.Estado),
                new SqlParameter("@Nombre", (object?)dto.Nombre ?? DBNull.Value),
                new SqlParameter("@Apellido", (object?)dto.Apellido ?? DBNull.Value),
                new SqlParameter("@Rut", (object?)dto.Rut ?? DBNull.Value),
                new SqlParameter("@Cargo", (object?)dto.Cargo ?? DBNull.Value),
                new SqlParameter("@Area", (object?)dto.Area ?? DBNull.Value),
                new SqlParameter("@Turno", (object?)dto.Turno ?? DBNull.Value),
                new SqlParameter("@HabitacionAsignada", (object?)dto.HabitacionAsignada ?? DBNull.Value),
                new SqlParameter("@FechaIngreso", (object?)dto.FechaIngreso ?? DBNull.Value),
                new SqlParameter("@FechaSalida", (object?)dto.FechaSalida ?? DBNull.Value)
            };

            try
            {
                _listaDotaciones.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool ActualizarDotacion(DotacionDto dto)
        {
            const string sql = "HOT_DOT_ACTUALIZAR @IdDotacion,@IdEmpresa,@Empresa,@Estado,@Nombre,@Apellido,@Rut,@Cargo,@Area,@Turno,@HabitacionAsignada,@FechaIngreso,@FechaSalida";
            var p = new SqlParameter[]
            {
                new SqlParameter("@IdDotacion", dto.IdDotacion),
                new SqlParameter("@IdEmpresa", (object?)dto.IdEmpresa ?? DBNull.Value),
                new SqlParameter("@Empresa", (object?)dto.Empresa ?? DBNull.Value),
                new SqlParameter("@Estado", dto.Estado),
                new SqlParameter("@Nombre", (object?)dto.Nombre ?? DBNull.Value),
                new SqlParameter("@Apellido", (object?)dto.Apellido ?? DBNull.Value),
                new SqlParameter("@Rut", (object?)dto.Rut ?? DBNull.Value),
                new SqlParameter("@Cargo", (object?)dto.Cargo ?? DBNull.Value),
                new SqlParameter("@Area", (object?)dto.Area ?? DBNull.Value),
                new SqlParameter("@Turno", (object?)dto.Turno ?? DBNull.Value),
                new SqlParameter("@HabitacionAsignada", (object?)dto.HabitacionAsignada ?? DBNull.Value),
                new SqlParameter("@FechaIngreso", (object?)dto.FechaIngreso ?? DBNull.Value),
                new SqlParameter("@FechaSalida", (object?)dto.FechaSalida ?? DBNull.Value)
            };

            try
            {
                _listaDotaciones.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool EliminarDotacion(int id)
        {
            const string sql = "HOT_DOT_ELIMINAR @IdDotacion";
            var p = new SqlParameter[] { new SqlParameter("@IdDotacion", id) };

            try
            {
                _listaDotaciones.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public DotacionKpiDto GetKpi()
        {
            var all = _listaDotaciones.GetAll().ToList();
            return new DotacionKpiDto
            {
                TotalTrabajadores = all.Count,
                TurnoDia = all.Count(x => x.Turno == "Día"),
                TurnoNoche = all.Count(x => x.Turno == "Noche"),
                FueraServicio = all.Count(x => !x.Estado)
            };
        }
    }
}
