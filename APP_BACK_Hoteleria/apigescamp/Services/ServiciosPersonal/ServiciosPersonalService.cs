using AutoMapper;
using DemoBackend.Dto.ServiciosPersonal;
using DemoBackend.Models.ServiciosPersonal;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.ServiciosPersonal;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DemoBackend.Services
{
    public class ServiciosPersonalService : IServiciosPersonalService
    {
        private readonly IGenericRepositoryEntity<ServiciosPersonalModels> _listaServicios;
        private readonly IMapper _mapper;
        //cambio 1-12
        public ServiciosPersonalService(
            IGenericRepositoryEntity<ServiciosPersonalModels> listaServicios,
            IMapper mapper)
        {
            _listaServicios = listaServicios;
            _mapper = mapper;
        }

        public ServiciosPersonalKpiDto GetKpi()
        {
            // Si hay SP: HOT_SPERS_KPI, úsalo. Si no, lo armo con la tabla.
            var all = _listaServicios.GetAll().ToList();
            return new ServiciosPersonalKpiDto
            {
                ServiciosActivos = all.Count(x => x.Estado == "ACTIVO"),
                ServiciosCompletados = all.Count(x => x.Estado == "COMPLETADO"),
                SolicitudesNuevas = all.Count(x => x.Estado == "NUEVO"),
                SolicitudesUrgentes = all.Count(x => x.Prioridad == "ALTA")
            };
        }

        public List<ServiciosPersonalDto> GetSolicitudes(string? estado)
        {
            const string sql = "HOT_SPERS_LISTAR @Estado";
            var p = new SqlParameter[] { new SqlParameter("@Estado", (object?)estado ?? DBNull.Value) };
            var lista = _listaServicios.GetStoreProcedure(sql, p);
            return _mapper.Map<List<ServiciosPersonalDto>>(lista);
        }

        public bool CrearSolicitud(ServiciosPersonalDto dto)
        {
            const string sql = "HOT_SPERS_CREAR @Tipo,@Descripcion,@Ubicacion,@Prioridad,@Estado,@FechaSolicitud,@FechaProgramada,@SolicitadoPor,@AsignadoA";
            var p = new SqlParameter[]
            {
                new SqlParameter("@Tipo", (object?)dto.Tipo ?? DBNull.Value),
                new SqlParameter("@Descripcion", (object?)dto.Descripcion ?? DBNull.Value),
                new SqlParameter("@Ubicacion", (object?)dto.Ubicacion ?? DBNull.Value),
                new SqlParameter("@Prioridad", (object?)dto.Prioridad ?? DBNull.Value),
                new SqlParameter("@Estado", (object?)dto.Estado ?? "NUEVO"),
                new SqlParameter("@FechaSolicitud", (object?)dto.FechaSolicitud ?? DBNull.Value),
                new SqlParameter("@FechaProgramada", (object?)dto.FechaProgramada ?? DBNull.Value),
                new SqlParameter("@SolicitadoPor", (object?)dto.SolicitadoPor ?? DBNull.Value),
                new SqlParameter("@AsignadoA", (object?)dto.AsignadoA ?? DBNull.Value),
            };

            try
            {
                _listaServicios.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool ActualizarSolicitud(ServiciosPersonalDto dto)
        {
            const string sql = "HOT_SPERS_ACTUALIZAR @Id,@Tipo,@Descripcion,@Ubicacion,@Prioridad,@Estado,@FechaSolicitud,@FechaProgramada,@SolicitadoPor,@AsignadoA";
            var p = new SqlParameter[]
            {
                new SqlParameter("@Id", dto.Id),
                new SqlParameter("@Tipo", (object?)dto.Tipo ?? DBNull.Value),
                new SqlParameter("@Descripcion", (object?)dto.Descripcion ?? DBNull.Value),
                new SqlParameter("@Ubicacion", (object?)dto.Ubicacion ?? DBNull.Value),
                new SqlParameter("@Prioridad", (object?)dto.Prioridad ?? DBNull.Value),
                new SqlParameter("@Estado", (object?)dto.Estado ?? DBNull.Value),
                new SqlParameter("@FechaSolicitud", (object?)dto.FechaSolicitud ?? DBNull.Value),
                new SqlParameter("@FechaProgramada", (object?)dto.FechaProgramada ?? DBNull.Value),
                new SqlParameter("@SolicitadoPor", (object?)dto.SolicitadoPor ?? DBNull.Value),
                new SqlParameter("@AsignadoA", (object?)dto.AsignadoA ?? DBNull.Value),
            };

            try
            {
                _listaServicios.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool CambiarEstado(int id, string estado)
        {
            const string sql = "HOT_SPERS_CAMBIA_ESTADO @Id,@Estado";
            var p = new SqlParameter[]
            {
                new SqlParameter("@Id", id),
                new SqlParameter("@Estado", (object?)estado ?? DBNull.Value)
            };

            try
            {
                _listaServicios.ExecuteProcedure(sql, p);
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
