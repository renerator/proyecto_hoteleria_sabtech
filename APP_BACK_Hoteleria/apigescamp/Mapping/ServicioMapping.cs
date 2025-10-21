using AutoMapper;

using DemoBackend.Dto.Servicio;
using DemoBackend.Models.Servicio;

namespace DemoBackend.Mapping
{
    public class ServicioMapping: Profile
    {
        public ServicioMapping()
        {
            CreateMap<ServicioDto, ServicioModels>();
            CreateMap<ServicioModels, ServicioDto>();

          
            
        }
    }
}
