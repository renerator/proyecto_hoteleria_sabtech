using AutoMapper;
using DemoBackend.Dto.Servicio;
using DemoBackend.Models.Servicio;

namespace DemoBackend.Mapping
{
    public class ServicioKPIMapping : Profile
    {
        public ServicioKPIMapping()
        {
            CreateMap<ServicioKpi, ServicioKpiDto>();
            CreateMap<ServicioKpiDto, ServicioKpi>();
        }
    }
}
