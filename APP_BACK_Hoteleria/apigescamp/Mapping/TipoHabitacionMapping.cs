using AutoMapper;

using DemoBackend.Dto.TipoHabitacion;
using DemoBackend.Models.TipoHabitacion;
using DemoBackend.Models.Trabajador;

namespace DemoBackend.Mapping
{
    public class TipoHabitacionMapping : Profile
    {
        public TipoHabitacionMapping()
        {
            CreateMap<TipoHabitacionDto, TipoHabitacionModels>();
            CreateMap<TipoHabitacionModels, TipoHabitacionDto>();



        }
    }
}