using DemoBackend.Dto.Servicio;
using System.Collections.Generic;

namespace DemoBackend.Services.Servicio
{
    public interface IServicioService
    {
        /// <summary>
        /// Obtiene la lista completa de servicios registrados.
        /// </summary>
        List<ServicioDto> GetListaServicio();

        /// <summary>
        /// Obtiene la lista de servicios filtrados por estado (1 = activos, 0 = inactivos).
        /// </summary>
        List<ServicioDto> GetListaServicioEstado(int estado);

        /// <summary>
        /// Crea un nuevo servicio en el sistema.
        /// </summary>
        bool CrearServicio(ServicioDto servicio);

        /// <summary>
        /// Modifica un servicio existente.
        /// </summary>
        bool ModificarServicio(ServicioDto servicio);

        /// <summary>
        /// Elimina (o desactiva) un servicio.
        /// </summary>
        bool EliminarServicio(ServicioDto servicio);

        /// <summary>
        /// Verifica si existe un servicio por su Id.
        /// </summary>
        List<ServicioDto> VerificaServicioPorId(ServicioDto servicio);
    }
}

