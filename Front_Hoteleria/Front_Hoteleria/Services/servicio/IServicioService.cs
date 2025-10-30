using Front_Hoteleria.Dto.Reserva;
using Front_Hoteleria.Dto.Servicio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Servicio
{
    public interface IServicioService
    {
        Task<List<ServicioDto>> ListarServiciosAsync(int? estado = null, string bearer = null);
        Task<bool> CrearServicioAsync(ServicioDto dto, string bearer = null);
        Task<bool> ModificarServicioAsync(ServicioDto dto, string bearer = null);
        Task<bool> EliminarServicioAsync(int idServicio, string bearer = null);
    }
}

