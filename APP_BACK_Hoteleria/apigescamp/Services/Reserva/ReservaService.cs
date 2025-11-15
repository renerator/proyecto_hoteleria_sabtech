using AutoMapper;
using DemoBackend.Dto.BitacoraReserva;
using DemoBackend.Dto.Reserva;
using DemoBackend.Dto.EstadoReserva;
using DemoBackend.Models.Reserva;
using DemoBackend.Models.EstadoReserva;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Reserva;

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DemoBackend.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IGenericRepositoryEntity<ReservaModels> _listaReserva;
        private readonly IGenericRepositoryEntity<ReservaDashboardKPI> _listaReservaDashboard;
        private readonly IGenericRepositoryEntity<ReservaDashboardPanelPrincipalModel> _listaReservaDashboardPanelPrincipal;
        private readonly IGenericRepositoryEntity<ReservaTrabajadorModels> _listaReservaTrabajador;
        private readonly IGenericRepositoryEntity<EstadoReservaModels> _listaEstadoReserva;
        private readonly IMapper _mapper;

        public ReservaService(
            IGenericRepositoryEntity<ReservaModels> listaReserva,
            IGenericRepositoryEntity<ReservaDashboardKPI> listaReservaDashboard,
            IGenericRepositoryEntity<ReservaTrabajadorModels> listaReservaTrabajador,
             IGenericRepositoryEntity<ReservaDashboardPanelPrincipalModel> listaReservaDashboardPanelPrincipal,
            IGenericRepositoryEntity<EstadoReservaModels> listaEstadoReserva,
        IMapper mapper)
        {
            _listaReserva = listaReserva;
            _listaReservaDashboard = listaReservaDashboard;
            _listaReservaTrabajador = listaReservaTrabajador;
            _listaReservaDashboardPanelPrincipal = listaReservaDashboardPanelPrincipal;
             _listaEstadoReserva = listaEstadoReserva;

            _mapper = mapper;
        }
        public bool CrearBitacoraReserva(BitacoraReservaDto dto)
        {
            if (dto == null) return false;
            if (dto.IdReserva <= 0) return false;

            const string sql = "HOT_RESBIT_CRE_BitacoraReserva @idReserva,@FechaBitacora,@idEstadoReserva,@Observaciones";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idReserva", dto.IdReserva),
                new SqlParameter("@FechaBitacora", (object?)dto.FechaBitacora ?? DBNull.Value),
                new SqlParameter("@idEstadoReserva", (object?)dto.IdEstadoReserva ?? DBNull.Value),
                new SqlParameter("@Observaciones", (object?)dto.Observaciones ?? DBNull.Value)
            };

            try
            {
                _listaReserva.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        #region Reservas
        public bool CrearReserva(ReservaDto reserva)
        {
            // IMPORTANTE: mismo nombre que el SP y mismos parámetros en el texto
            string sql = "HOT_CRE_Reserva " +
                         "@idHabitacion," +
                         "@IdReservaTipoHabitacion," +   // 🔹 nuevo en la cadena
                         "@FechaDesde," +
                         "@FechaHasta," +
                         "@QuiereTransporte," +
                         "@FechaCheckIN," +
                         "@FechaCheckOut," +
                         "@idEstadoReserva," +
                         "@Observaciones," +
                         "@NombreHuesped," +
                         "@RutHuesped," +
                         "@CorreoHuespedReserva," +
                         "@TelefonoHuespedReserva";

            // 13 parámetros (0..12) EXACTAMENTE los del SP
            var parametros = new SqlParameter[13];

            parametros[0] = new SqlParameter("@idHabitacion", reserva.IdHabitacion);
            parametros[1] = new SqlParameter("@IdReservaTipoHabitacion", reserva.IdReservaTipoHabitacion);

            parametros[2] = new SqlParameter("@FechaDesde", reserva.FechaDesde);
            parametros[3] = new SqlParameter("@FechaHasta", reserva.FechaHasta);

            // si QuiereTransporte es bool? en el DTO:
            bool quiere = reserva.QuiereTransporte is bool b ? b : false;
            parametros[4] = new SqlParameter("@QuiereTransporte", quiere);

            parametros[5] = new SqlParameter("@FechaCheckIN",
                                             (object?)reserva.FechaCheckIN ?? DBNull.Value);
            parametros[6] = new SqlParameter("@FechaCheckOut",
                                             (object?)reserva.FechaCheckOut ?? DBNull.Value);

            parametros[7] = new SqlParameter("@idEstadoReserva", reserva.IdEstadoReserva);

            parametros[8] = new SqlParameter("@Observaciones",
                                             (object?)reserva.Observaciones ?? DBNull.Value);

            parametros[9] = new SqlParameter("@NombreHuesped",
                                             (object?)reserva.NombreHuesped ?? DBNull.Value);

            parametros[10] = new SqlParameter("@RutHuesped",
                                              (object?)reserva.RutHuesped ?? DBNull.Value);

            parametros[11] = new SqlParameter("@CorreoHuespedReserva",
                                              (object?)reserva.CorreoHuespedReserva ?? DBNull.Value);

            parametros[12] = new SqlParameter("@TelefonoHuespedReserva",
                                              (object?)reserva.TelefonoHuespedReserva ?? DBNull.Value);

            try
            {
                _listaReserva.InsertProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
            parametros[9] = new SqlParameter("@Observaciones", (object?)reserva.Observaciones ?? DBNull.Value);

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
            var lista = _listaReserva.GetStoreProcedure(sql);


            return _mapper.Map<List<ReservaDto>>(lista);
        }
  
public List<ReservaDto> GetListaReservaEstado(int idEstadoReserva, DateTime? fechaDesde, DateTime? fechaHasta)
    {
        // Opción A: Si tu repo ejecuta por nombre de SP (CommandType.StoredProcedure)
        const string sql = "LISTADO_Reserva_Estado @idEstadoReserva,@fechaDesde,@fechaHasta";

        var parametros = new[]
        {
        new SqlParameter("@idEstadoReserva", SqlDbType.Int)      { Value = idEstadoReserva },
        new SqlParameter("@fechaDesde",      SqlDbType.DateTime) { Value = (object)fechaDesde ?? DBNull.Value, IsNullable = true },
        new SqlParameter("@fechaHasta",      SqlDbType.DateTime) { Value = (object)fechaHasta ?? DBNull.Value, IsNullable = true }
    };

        var lista = _listaReserva.GetStoreProcedure(sql, parametros);
        return _mapper.Map<List<ReservaDto>>(lista);
    }



    public List<ReservaDto> VerificaReservaPorId(ReservaDto reserva)
        {
            string sql = "HOT_VERIFICA_ID_RESERVA @idReserva";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@idReserva", reserva.IdReserva);

            var lista = _listaReserva.GetStoreProcedure(sql, parametros);


            return _mapper.Map<List<ReservaDto>>(lista);
        }

        #endregion

        public List<ReservaDto> BuscaReservas(ReservaDto reserva)
        {
            // Stored procedure que permite filtrar por múltiples campos
            string sql = "HOT_BUSCA_Reserva @idReserva, @idHabitacion, @idTrabajador, @FechaDesde, @FechaHasta, @QuiereTransporte, @FechaCheckIN, @FechaCheckOut, @idEstadoReserva, @MotivoReserva";

            var parametros = new SqlParameter[10];
            parametros[0] = new SqlParameter("@idReserva", reserva.IdReserva);
            parametros[1] = new SqlParameter("@idHabitacion", reserva.IdHabitacion);
            parametros[2] = new SqlParameter("@idTrabajador", reserva.IdTrabajador);
            parametros[3] = new SqlParameter("@FechaDesde", (object?)reserva.FechaDesde ?? DBNull.Value);
            parametros[4] = new SqlParameter("@FechaHasta", (object?)reserva.FechaHasta ?? DBNull.Value);
            parametros[5] = new SqlParameter("@QuiereTransporte", (object?)reserva.QuiereTransporte ?? DBNull.Value);
            parametros[6] = new SqlParameter("@FechaCheckIN", (object?)reserva.FechaCheckIN ?? DBNull.Value);
            parametros[7] = new SqlParameter("@FechaCheckOut", (object?)reserva.FechaCheckOut ?? DBNull.Value);
            parametros[8] = new SqlParameter("@idEstadoReserva", reserva.IdEstadoReserva);
            parametros[9] = new SqlParameter("@MotivoReserva", (object?)reserva.Observaciones ?? DBNull.Value);

            var lista = _listaReserva.GetStoreProcedure(sql, parametros);

            return _mapper.Map<List<ReservaDto>>(lista);
        }

        public ReservaDashboardDto ObtenerDashboard()
        {
            //var hoy = DateTime.Today;
            //var d = (desde ?? hoy).Date;
            //var h = (hasta ?? hoy).Date;

            // Nombre del SP correcto
            string sql = "HOT_DASH_TotalesYEstados";
            var parametros = new SqlParameter[0];
            //parametros[0] = new SqlParameter("@Desde", (object?)desde ?? DBNull.Value);
            //parametros[1] = new SqlParameter("@Hasta", (object?)hasta ?? DBNull.Value);
            //parametros[2] = new SqlParameter("@idHabitacion", idHabitacion);
            //parametros[3] = new SqlParameter("@idTipoReserva", idTipoReserva);

            var dto = new ReservaDashboardDto();

            try
            {
                var kpiRow = _listaReservaDashboard.GetStoreProcedure(sql, parametros).FirstOrDefault();
                if (kpiRow != null)
                {
                    dto.ReservasPendientes = kpiRow.ReservasPendientes;
                    dto.ReservasRechazadas = kpiRow.ReservasRechazadas;
                    //dto.TotalServicios = kpiRow.TotalServicios;
                    //dto.NuevasHoy = kpiRow.NuevasHoy;


                }



            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerDashboard: {ex.Message}");
            }

            return dto;
        }
        public ReservaDashboardPanelPrincipaDto ObtenerDashboardPanelPrincipal(DateTime? desde, DateTime? hasta)
        {
                       

            var dto = new ReservaDashboardPanelPrincipaDto();

            try
            {
                string sql = "HOT_DASH_PANELPRINCIPAL @Desde, @Hasta";
                var parametros = new SqlParameter[2];
                parametros[0] = new SqlParameter("@Desde", (object?)desde ?? DBNull.Value);
                parametros[1] = new SqlParameter("@Hasta", (object?)hasta ?? DBNull.Value);
                var kpiRow = _listaReservaDashboardPanelPrincipal.GetStoreProcedure(sql, parametros).FirstOrDefault();
                if (kpiRow != null)
                {
                    dto.NuevasReservas = kpiRow.NuevasReservas;
                    dto.Checkin = kpiRow.Checkin;
                    dto.Checkout = kpiRow.Checkout;
                    dto.Servicios = kpiRow.Servicios;
                   
                   
                    dto.Labels = kpiRow.Labels;
                    dto.Values= kpiRow.Values;




                }



            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerDashboard: {ex.Message}");
            }

            return dto;
        }





        public List<ReservaTrabajadorDto> GetListaReservaTrabajador(ReservaTrabajadorDto filtro)
        {
            // Evita NRE y permite filtros vacíos
            filtro ??= new ReservaTrabajadorDto();

            const string sql = "LISTADO_ReservasTrabajador @FechaDesde, @FechaHasta, @idEstadoReserva, @idTipoReserva";

            var parametros = new SqlParameter[4];
            {
                parametros[0] = new SqlParameter("@FechaDesde", (object?)filtro.FechaDesde ?? DBNull.Value);
                parametros[1] = new SqlParameter("@FechaHasta", (object?)filtro.FechaHasta ?? DBNull.Value);
                // Si 0 significa “no filtrar”, lo enviamos como NULL al SP
                parametros[2] = new SqlParameter("@idEstadoReserva", filtro.IdEstadoReserva);
                parametros[3] = new SqlParameter("@idTipoReserva", filtro.IdTipoReserva);


                try
                {
                    var lista = _listaReservaTrabajador.GetStoreProcedure(sql, parametros);

                    // Si AutoMapper no tiene el mapeo apropiado, lanzará excepción (capturada abajo)
                    var result = _mapper.Map<List<ReservaTrabajadorDto>>(lista);

                    return result ?? new List<ReservaTrabajadorDto>();
                }
                catch (AutoMapperMappingException amex)
                {
                    Console.WriteLine($"[GetListaReservaTrabajador] Error de mapeo: {amex.Message}");
                    if (amex.InnerException != null)
                        Console.WriteLine($"[GetListaReservaTrabajador] Inner: {amex.InnerException.Message}");
                    return new List<ReservaTrabajadorDto>();
                }
                catch (SqlException sqlex)
                {
                    Console.WriteLine($"[GetListaReservaTrabajador] SQL ({sqlex.Number}): {sqlex.Message}");
                    return new List<ReservaTrabajadorDto>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GetListaReservaTrabajador] Error inesperado: {ex}");
                    return new List<ReservaTrabajadorDto>();
                }



            }
        }



        public int CreaReservaTrabajador(ReservaTrabajadorDto dto)
        {
            if (dto == null) return 0;

            // Si no envían estado, por defecto 1=Ingresada
            if (dto.IdEstadoReserva <= 0) dto.IdEstadoReserva = 1;

            const string sql = "hot_Crea_Upd_Reserva @IdReserva OUTPUT, @idHabitacion, @idTrabajador, " +
                               "@FechaDesde, @FechaHasta, @QuiereTransporte, @FechaCheckIN, @FechaCheckOut, " +
                               "@idEstadoReserva, @MotivoReserva, @Totales";

            var parametros = new SqlParameter[11];
            parametros[0] = new SqlParameter("@IdReserva", SqlDbType.Int)
            {
                Direction = ParameterDirection.InputOutput,
                Value = (dto.IdReserva > 0 ? dto.IdReserva : 0)
            };
            parametros[1] = new SqlParameter("@idHabitacion", dto.IdHabitacion);
            parametros[2] = new SqlParameter("@idTrabajador", dto.IdTrabajador);
            parametros[3] = new SqlParameter("@FechaDesde", (object?)dto.FechaDesde ?? DBNull.Value);
            parametros[4] = new SqlParameter("@FechaHasta", (object?)dto.FechaHasta ?? DBNull.Value);
            parametros[5] = new SqlParameter("@QuiereTransporte", (object?)dto.QuiereTransporte ?? DBNull.Value);
            parametros[6] = new SqlParameter("@FechaCheckIN", (object?)dto.FechaCheckIN ?? DBNull.Value);
            parametros[7] = new SqlParameter("@FechaCheckOut", (object?)dto.FechaCheckOut ?? DBNull.Value);
            parametros[8] = new SqlParameter("@idEstadoReserva", dto.IdEstadoReserva);
            parametros[9] = new SqlParameter("@MotivoReserva", (object?)dto.MotivoReserva ?? DBNull.Value);
            parametros[10] = new SqlParameter("@Totales", (object?)dto.Totales ?? DBNull.Value);

            try
            {

                if (dto.IdReserva == 0)
                {
                    _listaReservaTrabajador.InsertProcedure(sql, parametros);

                    var val = parametros[0].Value;
                    int idReserva = 0;
                    if (val != null && val != DBNull.Value)
                        idReserva = Convert.ToInt32(val);

                    return idReserva; // >0 OK
                }
                else{
                    
                    _listaReserva.ExecuteProcedure(sql, parametros);

                    return 0; // >0 OK

                }
                // Ejecuta el SP; si tu repositorio NO propaga OUTPUT, cambia a ADO.NET plano.
                
            }
            catch (SqlException sqlex)
            {
                Console.WriteLine($"[CreaReservaTrabajador] SQL ({sqlex.Number}): {sqlex.Message}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreaReservaTrabajador] Error: {ex}");
                return 0;
            }
        }

        public List<EstadoReservaDto> GetListaEstadoReserva()
        {
            const string sql = "HOT_ESTADO_RESERVA_LISTAR";
            var respuesta = new List<EstadoReservaDto>();

            try
            {
                var lista = _listaEstadoReserva.GetStoreProcedure(sql, Array.Empty<SqlParameter>());
                if (lista == null || !lista.Any()) return respuesta;

                return _mapper.Map<List<EstadoReservaDto>>(lista);
                // Si tu repo ya devuelve DTO:
                // return lista.Cast<EstadoReservaDto>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return respuesta; // vacío ante error
            }
        }


    }

}