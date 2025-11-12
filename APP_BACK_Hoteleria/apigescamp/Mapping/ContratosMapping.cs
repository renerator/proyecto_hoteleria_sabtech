using AutoMapper;
using DemoBackend.Dto.Contratos;
using DemoBackend.Models.Contratos;

namespace DemoBackend.Mapping
{
    public class ContratosMapping : Profile
    {
        public ContratosMapping()
        {
            CreateMap<ContratosModels, ContratoDto>().ReverseMap();
            CreateMap<ContratoTrabajadoresModels, ContratoTrabajadorDto>().ReverseMap();
        }
    }
}
