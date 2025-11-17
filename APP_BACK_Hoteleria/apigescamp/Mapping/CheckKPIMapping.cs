using AutoMapper;

using DemoBackend.Dto.Check;
using DemoBackend.Models.Check;

namespace DemoBackend.Mapping
{
    public class CheckKPIMapping : Profile
    {
        public CheckKPIMapping()
        {
            CreateMap<CheckKPIDTO, CheckKPIModels>();
            CreateMap<CheckKPIModels, CheckKPIDTO>();

          
            
        }
    }
}
