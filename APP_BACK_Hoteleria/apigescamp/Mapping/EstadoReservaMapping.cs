using AutoMapper;

using DemoBackend.Dto.EstadoReserva;
using DemoBackend.Models.EstadoReserva;
using DemoBackend.Dto.Trabajador;

namespace DemoBackend.Mapping
{
    public class EstadoReservaMapping : Profile
    {
        public EstadoReservaMapping()
        {
            CreateMap<EstadoReservaDto, EstadoReservaModels>();
            CreateMap<EstadoReservaModels, EstadoReservaDto>();



        }
    }
}