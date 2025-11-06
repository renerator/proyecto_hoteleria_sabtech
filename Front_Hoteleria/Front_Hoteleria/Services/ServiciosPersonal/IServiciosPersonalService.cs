using Front_Hoteleria.Dto.ServiciosPersonal;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.ServiciosPersonal
{
    public interface IServiciosPersonalService
    {
        Task<ServiciosPersonalKpiDto> ObtenerKpiAsync(string bearer = null);

        // solicitudes pendientes (la tabla grande)
        Task<List<ServiciosPersonalDto>> ListarSolicitudesAsync(
            string ordenarPor = null,
            string prioridad = null,
            string estado = null,
            string ubicacion = null,
            string bearer = null);

        // panel izquierdo
        Task<List<ServiciosPersonalDto>> ListarServiciosActivosAsync(string bearer = null);

        // panel derecho
        Task<List<ServiciosPersonalDto>> ListarProximasSolicitudesAsync(string bearer = null);

        // acciones
        Task<bool> AsignarSolicitudAsync(string id, string bearer = null);
        Task<bool> IniciarSolicitudAsync(string id, string tiempoEstimado, string observaciones, string bearer = null);
        Task<bool> CompletarServicioAsync(string id, string descripcion, string bearer = null);
        Task<bool> NotificarHuespedAsync(string id, string metodo, string destino, string mensaje, string bearer = null);
    }
}
