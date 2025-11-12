using AutoMapper;
using DemoBackend.Dto.ServiciosPersonal;
using DemoBackend.Models.ServiciosPersonal;

namespace DemoBackend.Mapping
{
    public class ServiciosPersonalMapping : Profile
    {
        public ServiciosPersonalMapping()
        {
            CreateMap<ServiciosPersonalModels, ServiciosPersonalDto>().ReverseMap();
        }
    }
}
