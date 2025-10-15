using AutoMapper;

using DemoBackend.Dto.Servicio;
using DemoBackend.Dto.SolicitudServicio;
using DemoBackend.Models.Servicio;
using DemoBackend.Models.SolicitudServicio;

namespace DemoBackend.Mapping
{
    public class SolicitudServicioMapping : Profile
    {
        public SolicitudServicioMapping()
        {
            CreateMap<SolicitudServicioDto, SolicitudServicioModels>();
            CreateMap<SolicitudServicioModels, SolicitudServicioDto>();



        }
    }
}

