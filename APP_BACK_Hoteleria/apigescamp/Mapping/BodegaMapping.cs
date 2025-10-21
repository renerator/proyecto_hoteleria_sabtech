using AutoMapper;
using DemoBackend.Dto.Bodega;

using DemoBackend.Models.Bodega;

namespace DemoBackend.Mapping
{
    public class BodegaMapping : Profile
    {
        public BodegaMapping()
        {
            CreateMap<BodegaDto, BodegaModels>();
            CreateMap<BodegaModels, BodegaDto>();
        }
    }
}
