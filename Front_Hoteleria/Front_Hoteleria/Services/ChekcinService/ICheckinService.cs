using Front_Hoteleria.Dto.Checkin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Checkin
{
    public interface ICheckinService
    {
        Task<List<ReservaCheckinDto>> ListarReservasAsync(DateTime? fecha, string estado, string bearer = null);
        Task<CheckinKpiDto> KpiAsync(DateTime? fecha, string bearer = null);

        Task<bool> HacerCheckinAsync(CheckinAccionDto dto, string bearer = null);
        Task<bool> HacerCheckoutAsync(CheckinAccionDto dto, string bearer = null);
        Task<bool> RegistrarNoShowAsync(CheckinAccionDto dto, string bearer = null);
        Task<bool> ExtenderReservaAsync(CheckinExtensionDto dto, string bearer = null);
    }
}
