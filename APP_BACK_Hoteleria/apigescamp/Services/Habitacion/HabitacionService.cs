using AutoMapper;
using DemoBackend.Dto.BitacoraHabitacion;
using DemoBackend.Dto.Habitacion;
using DemoBackend.Dto.TipoHabitacion;
using DemoBackend.Dto.Reserva;
using DemoBackend.Models.TipoHabitacion;
using DemoBackend.Models.Habitacion;
using DemoBackend.Models.Reserva;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Habitacion;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoBackend.Services
{
    public class HabitacionService : IHabitacionService
    {
        private readonly IGenericRepositoryEntity<HabitacionModels> _listaHabitacion;
        private readonly IGenericRepositoryEntity<HabitacionDashboardModels> _listaHabitacionDashboard;
        private readonly IGenericRepositoryEntity<TipoHabitacionModels> _listaTipoHabitacion;
        private readonly IMapper _mapper;

        public HabitacionService(
            IGenericRepositoryEntity<HabitacionModels> listaHabitacion,
            IGenericRepositoryEntity<HabitacionDashboardModels> listaReservaDashboard,
            IGenericRepositoryEntity<TipoHabitacionModels> listaTipoHabitacion,
        IMapper mapper)
        {
            _listaTipoHabitacion = listaTipoHabitacion;
            _listaHabitacion = listaHabitacion;
            _listaHabitacionDashboard = listaReservaDashboard;
            _mapper = mapper;
        }

        #region Habitacion
        public bool CrearHabitacion(HabitacionDto habitacion)
        {
            string sql = "MAN_CRE_Habitacion @idArea,@NombreHabitacion,@Capacidad,@VIP,@idEstado,@idEmpresa,@Motivo,@idTipoHabitacion";
            var parametros = new SqlParameter[8];
            parametros[0] = new SqlParameter("@idArea", habitacion.IdArea);
            parametros[1] = new SqlParameter("@NombreHabitacion", habitacion.NombreHabitacion);
            parametros[2] = new SqlParameter("@Capacidad", habitacion.Capacidad);
            parametros[3] = new SqlParameter("@VIP", habitacion.VIP);
            parametros[4] = new SqlParameter("@idEstado", habitacion.IdEstado);
            parametros[5] = new SqlParameter("@idEmpresa", habitacion.IdEmpresa); 
            parametros[6] = new SqlParameter("@Motivo", (object?)habitacion.Motivo ?? DBNull.Value);
            parametros[7] = new SqlParameter("@Motivo", (object?)habitacion.IdTipoHabitacion ?? DBNull.Value);

            try
            {
                _listaHabitacion.InsertProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }

        public bool ModificarHabitacion(HabitacionDto habitacion)
        {
            string sql = "MAN_UPD_Habitacion @idHabitacion,@idArea,@NombreHabitacion,@Capacidad,@VIP,@idEstado,@idEmpresa,@Motivo,@idTipoHabitacion";
            var parametros = new SqlParameter[9];
            parametros[0] = new SqlParameter("@idHabitacion", habitacion.IdHabitacion);
            parametros[1] = new SqlParameter("@idArea", habitacion.IdArea);
            parametros[2] = new SqlParameter("@NombreHabitacion", habitacion.NombreHabitacion);
            parametros[3] = new SqlParameter("@Capacidad", habitacion.Capacidad);
            parametros[4] = new SqlParameter("@VIP", habitacion.VIP);
            parametros[5] = new SqlParameter("@idEstado", habitacion.IdEstado);
            parametros[6] = new SqlParameter("@idEmpresa", habitacion.IdEmpresa);
            parametros[7] = new SqlParameter("@Motivo", (object?)habitacion.Motivo ?? DBNull.Value);
            parametros[8] = new SqlParameter("@Motivo", (object?)habitacion.IdTipoHabitacion ?? DBNull.Value);
            try
            {
                _listaHabitacion.ExecuteProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }

        public bool EliminarHabitacion(HabitacionDto habitacion)
        {
            string sql = "MAN_DEL_Habitacion @idHabitacion";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@idHabitacion", habitacion.IdHabitacion);

            try
            {
                _listaHabitacion.ExecuteProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }


        public List<HabitacionDto> GetListaHabitacion()
        {
            string sql = "LISTADO_Habitacion";
            var listagrupos = _listaHabitacion.GetStoreProcedure(sql);


            return _mapper.Map<List<HabitacionDto>>(listagrupos);
        }



        public List<HabitacionDto> GetListaHabitacionEstado(int Vigente)
        {
            string sql = "LISTADO_Habitacion_Estado @Vigencia";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Vigencia", Vigente);

            var listagrupos = _listaHabitacion.GetStoreProcedure(sql, parametros);


            return _mapper.Map<List<HabitacionDto>>(listagrupos);
        }

        public HabitacionDashboardDto ObtenerDashboardHabitacion()
        {
            const string sql = "DASH_ResumenHabitaciones"; // SP sin parámetros
            var dto = new HabitacionDashboardDto();

            if (_listaHabitacionDashboard == null)
                throw new InvalidOperationException("_listaHabitacionDashboard es null (no inyectado en DI).");

            var rows = _listaHabitacionDashboard.GetStoreProcedure(sql, Array.Empty<SqlParameter>());
            var k = rows?.FirstOrDefault();
            if (k != null)
            {
                dto.HabitacionesHabilitadas = k.HabitacionesHabilitadas;
                dto.HabitacionesMantencion = k.HabitacionesMantencion;
                dto.HabitacionesOcupadas = k.HabitacionesOcupadas;
                dto.ServiciosSolicitados = k.ServiciosSolicitados;
                dto.AseoEnCurso = k.AseoEnCurso;
                if (k.GetType().GetProperty("ServiciosVarPorcentaje") != null);// dto.ServiciosVarPorcentaje = k.ServiciosVarPorcentaje;
                if (k.GetType().GetProperty("HuespedesRegistrados") != null);// dto.HuespedesRegistrados = k.HuespedesRegistrados;
            }
            return dto;
        }


        public List<HabitacionDto> VerificaHabitacionPorNombre(HabitacionDto habitacion)
        {
            string sql = "MAN_VERIFICA_HABITACION @NombreHabitacion";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@NombreHabitacion", habitacion.NombreHabitacion);

            var listagrupos = _listaHabitacion.GetStoreProcedure(sql, parametros);


            return _mapper.Map<List<HabitacionDto>>(listagrupos);
        }


        public List<HabitacionDto> VerificaHabitacionPorId(HabitacionDto habitacion)
        {
            string sql = "MAN_VERIFICA_ID_HABITACION @ID";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@ID", habitacion.IdHabitacion);

            var listagrupos = _listaHabitacion.GetStoreProcedure(sql, parametros);


            return _mapper.Map<List<HabitacionDto>>(listagrupos);
        }


        public bool CrearBitacoraHabitacion(BitacoraHabitacionDto dto)
        {
            if (dto == null) return false;
            if (dto.IdHabitacion <= 0) return false;

            const string sql = "HOT_BIT_CRE_BitacoraHabitacion @idHabitacion,@FechaBitacora,@TipoBitacora";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idHabitacion",   dto.IdHabitacion),
                new SqlParameter("@FechaBitacora", (object?)dto.FechaBitacora ?? DBNull.Value),
                new SqlParameter("@TipoBitacora",  (object?)dto.TipoBitacora  ?? DBNull.Value),
            };

            try
            {
                _listaHabitacion.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        // === SIN PARÁMETROS, MISMO ESTILO QUE GetListaMenu ===
        public List<TipoHabitacionDto> GetListaTipoHabitacion()
        {
            const string sql = "HOT_TIPO_HAB_LISTAR";
            var respuesta = new List<TipoHabitacionDto>();

            try
            {
                // Sin parámetros
                var lista = _listaTipoHabitacion.GetStoreProcedure(sql, Array.Empty<SqlParameter>());

                if (lista == null || !lista.Any())
                    return respuesta;

                // Si el repo retorna entidades, mapeamos a DTO
                return _mapper.Map<List<TipoHabitacionDto>>(lista);

                // Si tu repo ya retorna DTO, usa:
                // return lista.Cast<TipoHabitacionDto>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return respuesta; // vacío ante error
            }
        }


        #endregion
    }
}
