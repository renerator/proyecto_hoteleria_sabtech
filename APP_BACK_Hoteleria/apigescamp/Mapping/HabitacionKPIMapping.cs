using AutoMapper;

using DemoBackend.Dto.Habitacion;
using DemoBackend.Models.Habitacion;

namespace DemoBackend.Mapping
{
    public class HabitacionKPIMapping: Profile
    {
        public HabitacionKPIMapping()
        {
            CreateMap<HabitacionDashboardDto, HabitacionDashboardModels>();
            CreateMap<HabitacionDashboardModels, HabitacionDashboardDto>();

          
            
        }
    }
}
