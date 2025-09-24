using AutoMapper;
using DemoBackend.Dto.Habitacion;
using DemoBackend.Models.Habitacion;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Habitacion;

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace DemoBackend.Services
{
    public class HabitacionService : IHabitacionService
    {
        private readonly IGenericRepositoryEntity<HabitacionModels> _listaHabitacion;
        private readonly IMapper _mapper;

        public HabitacionService(
            IGenericRepositoryEntity<HabitacionModels> listaHabitacion,
            IMapper mapper)
        {
            _listaHabitacion = listaHabitacion;
            _mapper = mapper;
        }

        #region Habitacion
        public bool CrearHabitacion(HabitacionDto habitacion)
        {
            string sql = "MAN_CRE_Habitacion @idArea,@NombreHabitacion,@Capacidad,@VIP,@idEstado,@idEmpresa,@Motivo";
            var parametros = new SqlParameter[7];
            parametros[0] = new SqlParameter("@idArea", habitacion.IdArea);
            parametros[1] = new SqlParameter("@NombreHabitacion", habitacion.NombreHabitacion);
            parametros[2] = new SqlParameter("@Capacidad", habitacion.Capacidad);
            parametros[3] = new SqlParameter("@VIP", habitacion.VIP);
            parametros[4] = new SqlParameter("@idEstado", habitacion.IdEstado);
            parametros[5] = new SqlParameter("@idEmpresa", habitacion.IdEmpresa);
            parametros[6] = new SqlParameter("@Motivo", (object?)habitacion.Motivo ?? DBNull.Value);

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
            string sql = "MAN_UPD_Habitacion @idHabitacion,@idArea,@NombreHabitacion,@Capacidad,@VIP,@idEstado,@idEmpresa,@Motivo";
            var parametros = new SqlParameter[8];
            parametros[0] = new SqlParameter("@idHabitacion", habitacion.IdHabitacion);
            parametros[1] = new SqlParameter("@idArea", habitacion.IdArea);
            parametros[2] = new SqlParameter("@NombreHabitacion", habitacion.NombreHabitacion);
            parametros[3] = new SqlParameter("@Capacidad", habitacion.Capacidad);
            parametros[4] = new SqlParameter("@VIP", habitacion.VIP);
            parametros[5] = new SqlParameter("@idEstado", habitacion.IdEstado);
            parametros[6] = new SqlParameter("@idEmpresa", habitacion.IdEmpresa);
            parametros[7] = new SqlParameter("@Motivo", (object?)habitacion.Motivo ?? DBNull.Value);

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
        #endregion
    }
}
