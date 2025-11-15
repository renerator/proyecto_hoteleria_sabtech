// OJO: tu DTO está en Font_Hoteleria según lo que enviaste
using Font_Hoteleria.Dto.Trabajadores;
using Front_Hoteleria.Dto.Roles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Trabajadores
{
    public interface ITrabajadoresService
    {
        /// <summary>
        /// Crea un trabajador en la API (POST /api/Trabajador/CrearTrabajador).
        /// Devuelve true si la API respondió 200 y el cuerpo indica éxito.
        /// </summary>
        Task<bool> CrearAsync(TrabajadoresDto dto, string bearer = null);

        Task<List<TrabajadoresDto>> ListarAsync(int? IdEmpresa = null, string bearer = null);
        Task<List<TrabajadoresDto>> BuscarTrabajadorAsync(string rut = null, string bearer = null);
    }
}
