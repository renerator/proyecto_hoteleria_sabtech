using AutoMapper;

using DemoBackend.Dto.Reserva;
using DemoBackend.Models.Reserva;

namespace DemoBackend.Mapping
{
    public class ReservaKPIMapping: Profile
    {
        public ReservaKPIMapping()
        {
            CreateMap<ReservaDashboardDto, ReservaDashboardKPI>();
            CreateMap<ReservaDashboardKPI, ReservaDashboardDto>();

          
            
        }
    }
}
