using AutoMapper;

using DemoBackend.Dto.Trabajador;
using DemoBackend.Models.Trabajador;

namespace DemoBackend.Mapping
{
    public class TrabajadorMapping: Profile
    {
        public TrabajadorMapping()
        {
            CreateMap<TrabajadorDto, TrabajadorModels>();
            CreateMap<TrabajadorModels, TrabajadorDto>();

          
            
        }
    }
}
