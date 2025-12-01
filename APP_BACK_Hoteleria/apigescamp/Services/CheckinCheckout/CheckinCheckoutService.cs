using AutoMapper;

using DemoBackend.Dto.Check;

using DemoBackend.Models.Check;

using DemoBackend.RepositoryGes;
using DemoBackend.Services.Check;

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DemoBackend.Services
{
    public class CheckinCheckoutService : ICheckinCheckoutService
    {
        private readonly IGenericRepositoryEntity<CheckModels> _listaCheck;
        private readonly IGenericRepositoryEntity<CheckKPIModels> _listaCheckDashboard;
        //cambio 1-12
        private readonly IMapper _mapper;

        public CheckinCheckoutService(
            IGenericRepositoryEntity<CheckModels> listaCheck,
            IGenericRepositoryEntity<CheckKPIModels> listaCheckDashboard,
            
            IMapper mapper)
        {
            _listaCheck = listaCheck;
            _listaCheckDashboard = listaCheckDashboard;
           
            _mapper = mapper;
        }

        #region Checks

        /// <summary>
        /// Listado genérico por estado y rango de fechas (usa LISTADO_Check_Estado).
        /// </summary>
        public List<CheckDTO> GetListar(int idEstadoCheck, DateTime? fechaDesde)
        {
            const string sql = "LISTADO_Check @idEstadoCheck, @fechaDesde";

            var parametros = new[]
            {
        new SqlParameter("@idEstadoCheck", SqlDbType.Int)
        {
            Value = idEstadoCheck
        },
        new SqlParameter("@fechaDesde", SqlDbType.DateTime)
        {
            Value = (object)fechaDesde ?? DBNull.Value,
            IsNullable = true
        },
    };

            var lista = _listaCheck.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<CheckDTO>>(lista);
        }

        /// <summary>
        /// Método específico para Checkin/Checkout: filtra por fecha (día completo) e idEstado.
        /// idEstadoCheck = 0 => todos los estados (el SP ya debe manejar eso).
        /// </summary>


        public CheckKPIDTO ObtenerDashboard()
        {
            string sql = "HOT_DASH_CHECK";
            var parametros = Array.Empty<SqlParameter>();

            var dto = new CheckKPIDTO();

            try
            {
                var kpiRow = _listaCheckDashboard.GetStoreProcedure(sql, parametros).FirstOrDefault();
                if (kpiRow != null)
                {
                    dto.CheckinHoy = kpiRow.CheckinHoy;
                    dto.CheckoutHoy = kpiRow.CheckoutHoy;
                    dto.Extensiones= kpiRow.Extensiones;
                    dto.NoShow = kpiRow.NoShow;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerDashboard: {ex.Message}");
            }

            return dto;
        }

        #endregion
    }
}
