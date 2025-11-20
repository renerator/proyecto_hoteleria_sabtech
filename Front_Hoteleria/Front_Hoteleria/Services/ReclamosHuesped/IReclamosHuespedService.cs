using Front_Hoteleria.Dto.Huesped;
using Front_Hoteleria.Dto.Reserva;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.ReclamosHuesped
{
    public interface IReclamosHuespedService
    {


        Task<bool> CrearReclamoHuespedAsync(ReclamoSolicitudDto dto, string bearer);
        Task<List<ReclamoSolicitudDto>> ListarReclamosHuespedAsync(string bearer);
        Task<ReclamoSolicitudDto> ObtenerReclamoHuespedPorIdAsync(int idReclamoHuesped, string bearer);


    }
}
