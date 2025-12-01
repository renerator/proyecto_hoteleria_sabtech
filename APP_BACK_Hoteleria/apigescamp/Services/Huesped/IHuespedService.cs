using DemoBackend.Dto.Huesped;
using DemoBackend.Dto.Servicio;
using DemoBackend.Dto.ServicioCategoria;
using DemoBackend.Dto.ServicioEstado;
using DemoBackend.Dto.ServicioPrioridad;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoBackend.Services.Huesped
{
    public interface IHuespedService
    {
        Task<bool> CrearReclamoHuespedAsync(ReclamoSolicitudDto dto, string bearer);
        Task<List<ReclamoSolicitudDto>> ListarReclamosHuespedAsync(string bearer);

        ReclamoSolicitudDto ObtenerReclamoHuespedPorId(int idReclamoHuesped);
        //cambio 1-12

        List<ReservaHuespedDto> Buscar(ReservaHuespedDto filtro);
        ReservaHuespedDto ObtenerPorId(int idReserva);
        bool Crear(ReservaHuespedDto dto);
        bool Actualizar(ReservaHuespedDto dto);
        bool Eliminar(int idReserva);

        bool RegistrarEncuesta(EncuestaSatisfaccionDto dto);

        List<ServicioHuespedDto> BuscarServiciosHuesped(ServicioHuespedDto filtro);
        ServicioHuespedDto ObtenerServicioHuespedPorId(int idSolicitudServicio);
        int CrearServicioHuesped(ServicioHuespedDto dto);
        bool ActualizarServicioHuesped(ServicioHuespedDto dto);
        bool EliminarServicioHuesped(int idSolicitudServicio);


    }
}

