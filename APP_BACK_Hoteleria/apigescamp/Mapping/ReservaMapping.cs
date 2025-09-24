using AutoMapper;

using DemoBackend.Dto.Reserva;
using DemoBackend.Models.Reserva;

namespace DemoBackend.Mapping
{
    public class ReservaMapping: Profile
    {
        public ReservaMapping()
        {
            CreateMap<ReservaDto, ReservaModels>();
            CreateMap<ReservaModels, ReservaDto>();

          
            
        }
    }
}
