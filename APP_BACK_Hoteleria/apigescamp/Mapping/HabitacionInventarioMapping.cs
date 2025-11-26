using AutoMapper;
using DemoBackend.Dto.HabitacionInsumo;

using DemoBackend.Models.HabitacionInsumo;

namespace DemoBackend.Mapping
{
    public class HabitacionInventarioMapping : Profile
    {
        public HabitacionInventarioMapping()
        {
            CreateMap<HabitacionInventarioDto, HabitacionInventarioModels>();
            CreateMap<HabitacionInventarioModels, HabitacionInventarioDto>();
        }
    }
}
