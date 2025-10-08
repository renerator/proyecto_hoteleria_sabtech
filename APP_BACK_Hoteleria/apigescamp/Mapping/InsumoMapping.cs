using AutoMapper;
using DemoBackend.Dto.Insumos;
using DemoBackend.Models.Insumos;

namespace DemoBackend.Mapping
{
    public class InsumoMapping : Profile
    {
        public InsumoMapping()
        {
            CreateMap<InsumoDto, InsumoModels>();
            CreateMap<InsumoModels, InsumoDto>();
        }
    }
}
