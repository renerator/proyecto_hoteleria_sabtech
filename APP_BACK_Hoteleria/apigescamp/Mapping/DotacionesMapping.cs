using AutoMapper;
using DemoBackend.Dto.Dotaciones;
using DemoBackend.Models.Dotaciones;

namespace DemoBackend.Mapping
{
    public class DotacionesMapping : Profile
    {
        public DotacionesMapping()
        {
            CreateMap<DotacionesModels, DotacionDto>().ReverseMap();
        }
    }
}
