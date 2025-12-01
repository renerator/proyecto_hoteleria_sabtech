// DemoBackend/Services/Empresa/IEmpresaService.cs
using DemoBackend.Dto.Empresa;
using System.Collections.Generic;

namespace DemoBackend.Services.Empresa
{
    public interface IEmpresaService
    {//cambio 1-12
        /// <summary>
        /// Lista empresas (para combo) con opción de solo activas y filtro por nombre/texto.
        /// </summary>
        List<EmpresaDto> Listar(bool? soloActivas, string? filtro);

        /// <summary>
        /// Crea una empresa contratista.
        /// </summary>
        bool Crear(EmpresaCrearDto dto);
    }
}
