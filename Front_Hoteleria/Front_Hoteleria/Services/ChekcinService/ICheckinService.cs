using Front_Hoteleria.Dto.Checkin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Checkin
{
    public interface ICheckinService
    {
        // Listado para la tabla
        Task<List<ReservaCheckinDto>> ListarReservasAsync(
            DateTime? fecha,
            int idEstado,
            string bearer = null);

        // KPIs del dashboard
        Task<CheckinKpiDto> KpiAsync(
            DateTime? fecha,
            string bearer = null);

        // Acciones sobre la reserva
       
    }
}
