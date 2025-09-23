using AutoMapper;
using DemoBackend.Dto.Reserva;
using DemoBackend.Models.Reserva;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Reserva;

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace DemoBackend.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IGenericRepositoryEntity<ReservaModels> _listaReserva;
        private readonly IMapper _mapper;

        public ReservaService(
            IGenericRepositoryEntity<ReservaModels> listaReserva,
            IMapper mapper)
        {
            _listaReserva = listaReserva;
            _mapper = mapper;
        }

        #region Reservas
        public bool CrearReserva(ReservaDto reserva)
        {
            string sql = "HOT_CRE_Reserva @idHabitacion,@idTrabajador,@FechaDesde,@FechaHasta,@QuiereTransporte,@FechaCheckIN,@FechaCheckOut,@idEstadoReserva,@MotivoReserva";
            var parametros = new SqlParameter[9];
            parametros[0] = new SqlParameter("@idHabitacion", reserva.IdHabitacion);
            parametros[1] = new SqlParameter("@idTrabajador", reserva.IdTrabajador);
            parametros[2] = new SqlParameter("@FechaDesde", reserva.FechaDesde);
            parametros[3] = new SqlParameter("@FechaHasta", reserva.FechaHasta);
            parametros[4] = new SqlParameter("@QuiereTransporte", reserva.QuiereTransporte);
            parametros[5] = new SqlParameter("@FechaCheckIN", (object?)reserva.FechaCheckIN ?? DBNull.Value);
            parametros[6] = new SqlParameter("@FechaCheckOut", (object?)reserva.FechaCheckOut ?? DBNull.Value);
            parametros[7] = new SqlParameter("@idEstadoReserva", reserva.IdEstadoReserva);
            parametros[8] = new SqlParameter("@MotivoReserva", (object?)reserva.MotivoReserva ?? DBNull.Value);

            try
            {
                _listaReserva.InsertProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }

        public bool ModificarReserva(ReservaDto reserva)
        {
            string sql = "HOT_UPD_Reserva @idReserva,@idHabitacion,@idTrabajador,@FechaDesde,@FechaHasta,@QuiereTransporte,@FechaCheckIN,@FechaCheckOut,@idEstadoReserva,@MotivoReserva";
            var parametros = new SqlParameter[10];
            parametros[0] = new SqlParameter("@idReserva", reserva.IdReserva);
            parametros[1] = new SqlParameter("@idHabitacion", reserva.IdHabitacion);
            parametros[2] = new SqlParameter("@idTrabajador", reserva.IdTrabajador);
            parametros[3] = new SqlParameter("@FechaDesde", reserva.FechaDesde);
            parametros[4] = new SqlParameter("@FechaHasta", reserva.FechaHasta);
            parametros[5] = new SqlParameter("@QuiereTransporte", reserva.QuiereTransporte);
            parametros[6] = new SqlParameter("@FechaCheckIN", (object?)reserva.FechaCheckIN ?? DBNull.Value);
            parametros[7] = new SqlParameter("@FechaCheckOut", (object?)reserva.FechaCheckOut ?? DBNull.Value);
            parametros[8] = new SqlParameter("@idEstadoReserva", reserva.IdEstadoReserva);
            parametros[9] = new SqlParameter("@MotivoReserva", (object?)reserva.MotivoReserva ?? DBNull.Value);

            try
            {
                _listaReserva.ExecuteProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }

        public bool EliminarReserva(ReservaDto reserva)
        {
            string sql = "HOT_DEL_Reserva @idReserva";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@idReserva", reserva.IdReserva);

            try
            {
                _listaReserva.ExecuteProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }

        public List<ReservaDto> GetListaReserva()
        {
            string sql = "LISTADO_Reserva";

            List<ReservaDto> resultado = new List<ReservaDto>();
            var lista = _listaReserva.GetStoreProcedure(sql);

            foreach (var item in lista)
            {
                ReservaDto resu = new ReservaDto
                {
                    IdReserva = item.IdReserva,
                    IdHabitacion = item.IdHabitacion,
                    IdTrabajador = item.IdTrabajador,
                    FechaDesde = item.FechaDesde,
                    FechaHasta = item.FechaHasta,
                    QuiereTransporte = item.QuiereTransporte,
                    FechaCheckIN = item.FechaCheckIN,
                    FechaCheckOut = item.FechaCheckOut,
                    IdEstadoReserva = item.IdEstadoReserva,
                    MotivoReserva = item.MotivoReserva   
                };
                resultado.Add(resu);
            }
            return resultado;
        }

        public List<ReservaDto> GetListaReservaEstado(int idEstadoReserva)
        {
            string sql = "LISTADO_Reserva_Estado @idEstadoReserva";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@idEstadoReserva", idEstadoReserva);

            List<ReservaDto> resultado = new List<ReservaDto>();
            var lista = _listaReserva.GetStoreProcedure(sql, parametros);

            foreach (var item in lista)
            {
                ReservaDto resu = new ReservaDto
                {
                    IdReserva = item.IdReserva,
                    IdHabitacion = item.IdHabitacion,
                    IdTrabajador = item.IdTrabajador,
                    FechaDesde = item.FechaDesde,
                    FechaHasta = item.FechaHasta,
                    QuiereTransporte = item.QuiereTransporte,
                    FechaCheckIN = item.FechaCheckIN,
                    FechaCheckOut = item.FechaCheckOut,
                    IdEstadoReserva = item.IdEstadoReserva,
                    MotivoReserva = item.MotivoReserva   
                };
                resultado.Add(resu);
            }
            return resultado;
        }

        public List<ReservaDto> VerificaReservaPorId(ReservaDto reserva)
        {
            string sql = "HOT_VERIFICA_ID_RESERVA @idReserva";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@idReserva", reserva.IdReserva);

            List<ReservaDto> resultado = new List<ReservaDto>();
            var lista = _listaReserva.GetStoreProcedure(sql, parametros);

            foreach (var item in lista)
            {
                ReservaDto resu = new ReservaDto
                {
                    IdReserva = item.IdReserva,
                    IdHabitacion = item.IdHabitacion,
                    IdTrabajador = item.IdTrabajador,
                    FechaDesde = item.FechaDesde,
                    FechaHasta = item.FechaHasta,
                    QuiereTransporte = item.QuiereTransporte,
                    FechaCheckIN = item.FechaCheckIN,
                    FechaCheckOut = item.FechaCheckOut,
                    IdEstadoReserva = item.IdEstadoReserva,
                    MotivoReserva = item.MotivoReserva  
                };
                resultado.Add(resu);
            }
            return resultado;
        }
        #endregion
    }
}
