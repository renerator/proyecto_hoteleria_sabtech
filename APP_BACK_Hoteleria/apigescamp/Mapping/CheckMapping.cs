using AutoMapper;

using DemoBackend.Dto.Check;
using DemoBackend.Models.Check;

namespace DemoBackend.Mapping
{
    public class CheckMapping : Profile
    {
        public CheckMapping()
        {
            CreateMap<CheckDTO, CheckModels>();
            CreateMap<CheckModels, CheckDTO>();

          
            
        }
    }
}
