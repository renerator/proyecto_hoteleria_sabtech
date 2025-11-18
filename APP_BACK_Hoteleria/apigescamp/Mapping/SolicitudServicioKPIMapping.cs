using AutoMapper;

using DemoBackend.Dto.Servicio;
using DemoBackend.Dto.SolicitudServicio;
using DemoBackend.Models.Servicio;
using DemoBackend.Models.SolicitudServicio;

namespace DemoBackend.Mapping
{
    public class SolicitudServicioKPIMapping : Profile
    {
        public SolicitudServicioKPIMapping()
        {
            CreateMap<SolicitudKPIDto, SolicitudKPIModels>();
            CreateMap<SolicitudKPIModels, SolicitudKPIDto>();



        }
    }
}

