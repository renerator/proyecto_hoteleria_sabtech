using AutoMapper;

using DemoBackend.Dto.Habitacion;
using DemoBackend.Models.Habitacion;

namespace DemoBackend.Mapping
{
    public class HabitacionKPI: Profile
    {
        public HabitacionKPI()
        {
            CreateMap<HabitacionDashboardDto, HabitacionDashboardModels>();
            CreateMap<HabitacionDashboardModels, HabitacionDashboardDto>();

          
            
        }
    }
}
