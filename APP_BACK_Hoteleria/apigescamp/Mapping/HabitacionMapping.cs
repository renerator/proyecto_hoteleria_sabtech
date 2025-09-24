using AutoMapper;

using DemoBackend.Dto.Habitacion;
using DemoBackend.Models.Habitacion;

namespace DemoBackend.Mapping
{
    public class HabitacionMapping: Profile
    {
        public HabitacionMapping()
        {
            CreateMap<HabitacionDto, HabitacionModels>();
            CreateMap<HabitacionModels, HabitacionDto>();

          
            
        }
    }
}
