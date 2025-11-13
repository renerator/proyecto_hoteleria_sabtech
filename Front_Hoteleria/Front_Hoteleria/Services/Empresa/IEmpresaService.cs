using Front_Hoteleria.Dto.Empresa;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Empresa
{
    public interface IEmpresaService
    {
        /// <summary>
        /// Obtiene el combo de empresas desde la API.
        /// </summary>
        Task<List<EmpresaDto>> ListarComboAsync(
            bool? soloActivas = true,
            string filtro = null,
            string bearer = null);

        Task<bool> CrearAsync(EmpresaCrearPostDto dto, string bearer = null);
    }
}
