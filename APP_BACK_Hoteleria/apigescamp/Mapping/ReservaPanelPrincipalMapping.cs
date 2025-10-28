using AutoMapper;

using DemoBackend.Dto.Reserva;
using DemoBackend.Models.Reserva;

namespace DemoBackend.Mapping
{
    public class ReservaPanelPrincipalMapping : Profile
    {
        public ReservaPanelPrincipalMapping()
        {
            CreateMap<ReservaDashboardPanelPrincipaDto, ReservaDashboardPanelPrincipalModel>();
            CreateMap<ReservaDashboardPanelPrincipalModel, ReservaDashboardPanelPrincipaDto>();

          
            
        }
    }
}
