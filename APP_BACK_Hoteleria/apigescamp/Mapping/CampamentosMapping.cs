using AutoMapper;
using DemoBackend.Dto.Campamentos;
using DemoBackend.Models.Campamentos;
using DemoBackend.Models.Reserva;

namespace DemoBackend.Mapping
{
    public class CampamentosMapping : Profile
    {
        public CampamentosMapping()
        {
            CreateMap<CampamentosModels, CampamentoDto>().ReverseMap();
            CreateMap<CampamentoAreasModels, CampamentoAreaDto>().ReverseMap();
            CreateMap<CampamentoKPIModels, CampamentoKpiDto>().ReverseMap();
        }


    }
}
