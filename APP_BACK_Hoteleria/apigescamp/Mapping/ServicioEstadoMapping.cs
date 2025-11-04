using AutoMapper;

using DemoBackend.Dto.ServicioEstado;
using DemoBackend.Models.ServicioEstado;


namespace DemoBackend.Mapping
{
    public class ServicioEstadodMapping : Profile
    {
        public ServicioEstadodMapping()
        {
            CreateMap<ServicioEstadoDto, ServicioEstadoModels>();
            CreateMap<ServicioEstadoModels, ServicioEstadoDto>();

          
            
        }
    }
}
