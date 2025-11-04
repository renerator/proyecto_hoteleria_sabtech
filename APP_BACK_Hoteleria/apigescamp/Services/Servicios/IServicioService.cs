using DemoBackend.Dto.Servicio;
using DemoBackend.Dto.ServicioEstado;
using DemoBackend.Dto.ServicioCategoria;
using DemoBackend.Dto.ServicioPrioridad;
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
        List<ServicioEstadoDto> GetListaServicioEstado(int estado);

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



        // estos son los nuevos para los combos
        List<ServicioCategoriaDto> GetListaServiciosCategoria(int vigencia);
        List<ServicioPrioridadDto> GetListaServicioPrioridad(int vigencia);

        ServicioKpiDto GetKpiServicios();
    }
}

