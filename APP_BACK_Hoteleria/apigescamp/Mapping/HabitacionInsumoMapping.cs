using AutoMapper;
using DemoBackend.Dto.HabitacionInsumo;

using DemoBackend.Models.HabitacionInsumo;

namespace DemoBackend.Mapping
{
    public class HabitacionInsumoMapping : Profile
    {
        public HabitacionInsumoMapping()
        {
            CreateMap<HabitacionInsumoDto, HabitacionInsumoModels>();
            CreateMap<HabitacionInsumoModels, HabitacionInsumoDto>();
        }
    }
}
