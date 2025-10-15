using AutoMapper;
using DemoBackend.Dto.OrdenTrabajo;

using DemoBackend.Models.OrdenTrabajo;


namespace DemoBackend.Mapping
{
    public class OrdenTrabajoMapping : Profile
    {
        public OrdenTrabajoMapping()
        {
            CreateMap<OrdenTrabajoDto, OrdenTrabajoModels>();
            CreateMap<OrdenTrabajoModels, OrdenTrabajoDto>();
        }
    }
}
