using AutoMapper;
using DemoBackend.Dto.Calendario;
using DemoBackend.Models.Calendario;

namespace DemoBackend.Mapping
{
    public class CalendarioMapping : Profile
    {
        public CalendarioMapping()
        {
            CreateMap<CalendarioEventosModels, CalendarioEventoDto>().ReverseMap();
            CreateMap<CalendarioBloqueosModels, CalendarioBloqueoDto>().ReverseMap();
            CreateMap<CalendarioMantenimientosModels, CalendarioMantenimientoDto>().ReverseMap();
            CreateMap<CalendarioSanitizacionModels, CalendarioSanitizacionDto>().ReverseMap();
        }
    }
}
