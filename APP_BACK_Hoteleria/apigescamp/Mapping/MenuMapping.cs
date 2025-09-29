using AutoMapper;

using DemoBackend.Dto.Menu;
using DemoBackend.Models.Menu;

namespace DemoBackend.Mapping
{
    public class MenuMapping: Profile
    {
        public MenuMapping()
        {
            CreateMap<MenuDto, MenuModels>();
            CreateMap<MenuModels, MenuDto>();

          
            
        }
    }
}
