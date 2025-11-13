using AutoMapper;
using DemoBackend.Dto.Empresa;

using DemoBackend.Models.EmpresaContratista;

namespace DemoBackend.Mapping
{
    public class EmpresaContratistaMapping : Profile
    {
        public EmpresaContratistaMapping()
        {
            CreateMap<EmpresaContratistaModels, EmpresaDto>()
             .ForMember(d => d.IdEmpresa, m => m.MapFrom(s => s.idEmpresaContratista))
             .ForMember(d => d.Nombre, m => m.MapFrom(s => s.NombreEmpresaContratista))
            .ForMember(d => d.Rut, m => m.MapFrom(s => s.DNIEmpresaContratista));
        }
    }
}
