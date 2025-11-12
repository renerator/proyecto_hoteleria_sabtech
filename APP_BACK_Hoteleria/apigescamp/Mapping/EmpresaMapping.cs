using AutoMapper;
using DemoBackend.Dto.Empresa;

using DemoBackend.Models.Empresa;

namespace DemoBackend.Mapping
{
    public class EmpresaMapping : Profile
    {
        public EmpresaMapping()
        {
            CreateMap<EmpresaDto, EmpresaModels>();
            CreateMap<EmpresaModels, EmpresaDto>();
        }
    }
}
