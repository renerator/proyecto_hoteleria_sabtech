using AutoMapper;
using DemoBackend.Dto.Calendario;
using DemoBackend.Models.Calendario;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Calendario;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DemoBackend.Services
{
    public class CalendarioService : ICalendarioService
    {
        private readonly IGenericRepositoryEntity<CalendarioEventosModels> _listaEventos;
        private readonly IGenericRepositoryEntity<CalendarioBloqueosModels> _listaBloqueos;
        private readonly IGenericRepositoryEntity<CalendarioMantenimientosModels> _listaMantenciones;
        private readonly IGenericRepositoryEntity<CalendarioSanitizacionModels> _listaSanitizacion;
        private readonly IMapper _mapper;

        public CalendarioService(
            IGenericRepositoryEntity<CalendarioEventosModels> listaEventos,
            IGenericRepositoryEntity<CalendarioBloqueosModels> listaBloqueos,
            IGenericRepositoryEntity<CalendarioMantenimientosModels> listaMantenciones,
            IGenericRepositoryEntity<CalendarioSanitizacionModels> listaSanitizacion,
            IMapper mapper)
        {
            _listaEventos = listaEventos;
            _listaBloqueos = listaBloqueos;
            _listaMantenciones = listaMantenciones;
            _listaSanitizacion = listaSanitizacion;
            _mapper = mapper;
        }

        public List<CalendarioEventoDto> GetEventos(int? habitacionId, DateTime? desde, DateTime? hasta)
        {
            const string sql = "HOT_CAL_LIST_Eventos @HabitacionId, @Desde, @Hasta";
            var p = new SqlParameter[]
            {
                new SqlParameter("@HabitacionId", (object?)habitacionId ?? DBNull.Value),
                new SqlParameter("@Desde", (object?)desde ?? DBNull.Value),
                new SqlParameter("@Hasta", (object?)hasta ?? DBNull.Value)
            };

            var lista = _listaEventos.GetStoreProcedure(sql, p);
            return _mapper.Map<List<CalendarioEventoDto>>(lista);
        }

        public bool CrearEvento(CalendarioEventoDto dto)
        {
            const string sql = "HOT_CAL_INS_Evento @HabitacionId,@Titulo,@FechaInicio,@FechaFin,@Tipo,@Descripcion,@Color";
            var p = new SqlParameter[]
            {
                new SqlParameter("@HabitacionId", (object?)dto.HabitacionId ?? DBNull.Value),
                new SqlParameter("@Titulo", (object?)dto.Titulo ?? DBNull.Value),
                new SqlParameter("@FechaInicio", dto.FechaInicio),
                new SqlParameter("@FechaFin", dto.FechaFin),
                new SqlParameter("@Tipo", (object?)dto.Tipo ?? DBNull.Value),
                new SqlParameter("@Descripcion", (object?)dto.Descripcion ?? DBNull.Value),
                new SqlParameter("@Color", (object?)dto.Color ?? DBNull.Value)
            };

            try
            {
                _listaEventos.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool ActualizarEvento(CalendarioEventoDto dto)
        {
            const string sql = "HOT_CAL_UPD_Evento @Id,@HabitacionId,@Titulo,@FechaInicio,@FechaFin,@Tipo,@Descripcion,@Color,@Estado";
            var p = new SqlParameter[]
            {
                new SqlParameter("@Id", dto.Id),
                new SqlParameter("@HabitacionId", (object?)dto.HabitacionId ?? DBNull.Value),
                new SqlParameter("@Titulo", (object?)dto.Titulo ?? DBNull.Value),
                new SqlParameter("@FechaInicio", dto.FechaInicio),
                new SqlParameter("@FechaFin", dto.FechaFin),
                new SqlParameter("@Tipo", (object?)dto.Tipo ?? DBNull.Value),
                new SqlParameter("@Descripcion", (object?)dto.Descripcion ?? DBNull.Value),
                new SqlParameter("@Color", (object?)dto.Color ?? DBNull.Value),
                new SqlParameter("@Estado", 1)
            };

            try
            {
                _listaEventos.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool EliminarEvento(int id)
        {
            const string sql = "HOT_CAL_DEL_Evento @Id";
            var p = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };

            try
            {
                _listaEventos.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool CrearBloqueo(CalendarioBloqueoDto dto)
        {
            const string sql = "HOT_CAL_INS_Bloqueo @HabitacionId,@FechaInicio,@FechaFin,@Motivo";
            var p = new SqlParameter[]
            {
                new SqlParameter("@HabitacionId", dto.HabitacionId),
                new SqlParameter("@FechaInicio", dto.FechaInicio),
                new SqlParameter("@FechaFin", dto.FechaFin),
                new SqlParameter("@Motivo", (object?)dto.Motivo ?? DBNull.Value)
            };

            try
            {
                _listaBloqueos.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool CrearMantenimiento(CalendarioMantenimientoDto dto)
        {
            const string sql = "HOT_CAL_INS_Mantenimiento @HabitacionId,@FechaInicio,@DuracionDias,@Descripcion,@Responsable";
            var p = new SqlParameter[]
            {
                new SqlParameter("@HabitacionId", dto.HabitacionId),
                new SqlParameter("@FechaInicio", dto.FechaInicio),
                new SqlParameter("@DuracionDias", dto.DuracionDias),
                new SqlParameter("@Descripcion", (object?)dto.Descripcion ?? DBNull.Value),
                new SqlParameter("@Responsable", (object?)dto.Responsable ?? DBNull.Value)
            };

            try
            {
                _listaMantenciones.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool CrearSanitizacion(CalendarioSanitizacionDto dto)
        {
            const string sql = "HOT_CAL_INS_Sanitizacion @HabitacionId,@FechaInicio,@DuracionHoras,@Tipo,@Personal";
            var p = new SqlParameter[]
            {
                new SqlParameter("@HabitacionId", dto.HabitacionId),
                new SqlParameter("@FechaInicio", dto.FechaInicio),
                new SqlParameter("@DuracionHoras", dto.DuracionHoras),
                new SqlParameter("@Tipo", (object?)dto.Tipo ?? DBNull.Value),
                new SqlParameter("@Personal", (object?)dto.Personal ?? DBNull.Value)
            };

            try
            {
                _listaSanitizacion.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public CalendarioKpiDto GetKpi()
        {
            // si no tienes SP específico, se puede armar con las tablas
            try
            {
                var totalEventos = _listaEventos.GetAll().Count();
                var mant = _listaMantenciones.GetAll().Count();
                var sanit = _listaSanitizacion.GetAll().Count();

                return new CalendarioKpiDto
                {
                    TotalHabitaciones = totalEventos,
                    OcupadasHoy = totalEventos,
                    EnMantenimiento = mant,
                    EnSanitizacion = sanit
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new CalendarioKpiDto();
            }
        }
    }
}
