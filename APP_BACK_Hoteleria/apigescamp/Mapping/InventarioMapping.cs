using AutoMapper;
using DemoBackend.Dto.Inventario;
using DemoBackend.Models.Inventario;

namespace DemoBackend.Mapping
{
    public class InventarioMapping : Profile
    {
        public InventarioMapping()
        {
            CreateMap<InventarioModels, InventarioItemDto>()
                .ForMember(d => d.IdArticulo, o => o.MapFrom(s => s.IdArticulo))
                .ReverseMap()
                .ForMember(d => d.IdArticulo, o => o.MapFrom(s => s.IdArticulo));
        }
    }
}
