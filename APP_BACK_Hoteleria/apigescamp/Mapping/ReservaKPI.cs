using AutoMapper;

using DemoBackend.Dto.Reserva;
using DemoBackend.Models.Reserva;

namespace DemoBackend.Mapping
{
    public class ReservaKPI: Profile
    {
        public ReservaKPI()
        {
            CreateMap<ReservaDashboardDto, ReservaDashboardKPI>();
            CreateMap<ReservaDashboardKPI, ReservaDashboardDto>();

          
            
        }
    }
}
