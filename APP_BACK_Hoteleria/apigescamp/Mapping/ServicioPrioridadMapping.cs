using AutoMapper;

using DemoBackend.Dto.ServicioPrioridad;
using DemoBackend.Models.ServicioPrioridad;

namespace DemoBackend.Mapping
{
    public class ServicioPrioridadMapping : Profile
    {
        public ServicioPrioridadMapping()
        {
            CreateMap<ServicioPrioridadDto, ServicioPrioridadModels>();
            CreateMap<ServicioPrioridadModels, ServicioPrioridadDto>();

          
            
        }
    }
}
