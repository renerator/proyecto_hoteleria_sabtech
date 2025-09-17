using AutoMapper;

using DemoBackend.Dto.Mantenedores;
using DemoBackend.Models.Mantenedores;

namespace DemoBackend.Mapping
{
    public class MantenedorMapping: Profile
    {
        public MantenedorMapping()
        {
            CreateMap<AreasDto, AreasModels>();
            CreateMap<AreasModels, AreasDto>();

          
            
        }
    }
}
