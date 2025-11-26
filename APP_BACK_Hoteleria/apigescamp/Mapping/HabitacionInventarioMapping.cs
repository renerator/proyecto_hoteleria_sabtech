using AutoMapper;
using DemoBackend.Dto.HabitacionInventario;

using DemoBackend.Models.HabitacionInventario;

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
