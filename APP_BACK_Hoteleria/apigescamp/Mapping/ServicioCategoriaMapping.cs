using AutoMapper;

using DemoBackend.Dto.ServicioCategoria;
using DemoBackend.Models.ServicioCategoria;

namespace DemoBackend.Mapping
{
    public class ServicioCategoriaMapping : Profile
    {
        public ServicioCategoriaMapping()
        {
            CreateMap<ServicioCategoriaDto, ServicioCategoriaModels>();
            CreateMap<ServicioCategoriaModels, ServicioCategoriaDto>();

          
            
        }
    }
}
