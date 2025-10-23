using AutoMapper;

using DemoBackend.Dto.Reserva;
using DemoBackend.Models.Reserva;

namespace DemoBackend.Mapping
{
    public class ReservaTrabajadorMapping : Profile
    {
        public ReservaTrabajadorMapping()
        {
            CreateMap<ReservaTrabajadorDto, ReservaTrabajadorModels>();
            CreateMap<ReservaTrabajadorModels, ReservaTrabajadorDto>();

          
            
        }
    }
}
