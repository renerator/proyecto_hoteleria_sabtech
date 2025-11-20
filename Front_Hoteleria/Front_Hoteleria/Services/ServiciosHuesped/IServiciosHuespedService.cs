using Front_Hoteleria.Dto.Huesped;
using Front_Hoteleria.Dto.Reserva;
using Front_Hoteleria.Dto.Servicio;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.ServiciosHuesped
{
    public interface IServiciosHuespedService
    {
        Task<List<ServicioHuespedDto>> ListarServiciosHuespedAsync(ServicioHuespedDto filtro, string bearer = null);
        Task<ServicioHuespedDto> ObtenerServicioHuespedPorIdAsync(int idSolicitud, string bearer = null);
        Task<bool> CrearServicioHuespedAsync(ServicioHuespedDto dto, string bearer = null);
        Task<bool> ActualizarServicioHuespedAsync(ServicioHuespedDto dto, string bearer = null);
        Task<bool> EliminarServicioHuespedAsync(int idSolicitud, string bearer = null);
    }
}
