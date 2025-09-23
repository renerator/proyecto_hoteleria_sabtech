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

            List<HabitacionDto> resultado = new List<HabitacionDto>();
            var listagrupos = _listaHabitacion.GetStoreProcedure(sql);

            foreach (var item in listagrupos)
            {
                HabitacionDto resu = new HabitacionDto
                {
                    IdHabitacion = item.IdHabitacion,
                    IdArea = item.IdArea,
                    NombreHabitacion = item.NombreHabitacion,
                    Capacidad = item.Capacidad,
                    VIP = item.VIP,
                    IdEstado = item.IdEstado,
                    IdEmpresa = item.IdEmpresa,
                    Motivo = item.Motivo   
                };
                resultado.Add(resu);
            }
            return resultado;
        }

        public List<HabitacionDto> GetListaHabitacionEstado(int Vigente)
        {
            string sql = "LISTADO_Habitacion_Estado @Vigencia";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Vigencia", Vigente);

            List<HabitacionDto> resultado = new List<HabitacionDto>();
            var listagrupos = _listaHabitacion.GetStoreProcedure(sql, parametros);

            foreach (var item in listagrupos)
            {
                HabitacionDto resu = new HabitacionDto
                {
                    IdHabitacion = item.IdHabitacion,
                    IdArea = item.IdArea,
                    NombreHabitacion = item.NombreHabitacion,
                    Capacidad = item.Capacidad,
                    VIP = item.VIP,
                    IdEstado = item.IdEstado,
                    IdEmpresa = item.IdEmpresa,
                    Motivo = item.Motivo   
                };
                resultado.Add(resu);
            }
            return resultado;
        }

        public List<HabitacionDto> VerificaHabitacionPorNombre(HabitacionDto habitacion)
        {
            string sql = "MAN_VERIFICA_HABITACION @NombreHabitacion";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@NombreHabitacion", habitacion.NombreHabitacion);

            List<HabitacionDto> resultado = new List<HabitacionDto>();
            var listagrupos = _listaHabitacion.GetStoreProcedure(sql, parametros);

            foreach (var item in listagrupos)
            {
                HabitacionDto resu = new HabitacionDto
                {
                    IdHabitacion = item.IdHabitacion,
                    IdArea = item.IdArea,
                    NombreHabitacion = item.NombreHabitacion,
                    Capacidad = item.Capacidad,
                    VIP = item.VIP,
                    IdEstado = item.IdEstado,
                    IdEmpresa = item.IdEmpresa,
                    Motivo = item.Motivo   
                };
                resultado.Add(resu);
            }
            return resultado;
        }

        public List<HabitacionDto> VerificaHabitacionPorId(HabitacionDto habitacion)
        {
            string sql = "MAN_VERIFICA_ID_HABITACION @ID";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@ID", habitacion.IdHabitacion);

            List<HabitacionDto> resultado = new List<HabitacionDto>();
            var listagrupos = _listaHabitacion.GetStoreProcedure(sql, parametros);

            foreach (var item in listagrupos)
            {
                HabitacionDto resu = new HabitacionDto
                {
                    IdHabitacion = item.IdHabitacion,
                    IdArea = item.IdArea,
                    NombreHabitacion = item.NombreHabitacion,
                    Capacidad = item.Capacidad,
                    VIP = item.VIP,
                    IdEstado = item.IdEstado,
                    IdEmpresa = item.IdEmpresa,
                    Motivo = item.Motivo   
                };
                resultado.Add(resu);
            }
            return resultado;
        }
        #endregion
    }
}
