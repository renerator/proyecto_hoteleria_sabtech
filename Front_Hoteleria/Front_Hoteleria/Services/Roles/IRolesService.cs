using Front_Hoteleria.Dto.Roles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Roles
{
    public interface IRolesService
    {
        Task<RolesKpiDto> ResumenAsync(string bearer = null);
        Task<List<RolDto>> ListarAsync(string criterio = null, string bearer = null);
        Task<RolDto> ObtenerPorIdAsync(int id, string bearer = null);
        Task<bool> CrearAsync(RolDto dto, string bearer = null);
        Task<bool> ActualizarAsync(RolDto dto, string bearer = null);
        Task<bool> EliminarAsync(int id, string bearer = null);
    }
}
