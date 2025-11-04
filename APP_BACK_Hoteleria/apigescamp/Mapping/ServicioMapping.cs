using AutoMapper;
using DemoBackend.Dto.Servicio;
using DemoBackend.Models.Servicio;

namespace DemoBackend.Mapping
{
    public class ServicioMapping : Profile
    {
        public ServicioMapping()
        {
            CreateMap<ServicioModels, ServicioDto>();
            CreateMap<ServicioDto, ServicioModels>();
        }
    }
}
