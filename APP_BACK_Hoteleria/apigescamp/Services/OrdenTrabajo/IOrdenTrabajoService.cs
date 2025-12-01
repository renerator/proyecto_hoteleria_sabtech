using System;
using System.Collections.Generic;
using DemoBackend.Dto.OrdenTrabajo;

namespace DemoBackend.Services.OrdenTrabajo
{
    public interface IOrdenTrabajoService
    {//cambio 1-12
        List<OrdenTrabajoDto> Buscar(
            int? idOrdenTrabajo = null,
            int? idHabitacion   = null,
            string? numeroOT    = null,
            DateTime? desde     = null,
            DateTime? hasta     = null);

        OrdenTrabajoDto? ObtenerPorId(int idOrdenTrabajo);
        bool Crear(OrdenTrabajoDto dto);
        bool Modificar(OrdenTrabajoDto dto);
        bool Eliminar(int idOrdenTrabajo);

        // Patrón de vigencia/estado (igual a otros servicios)
        List<OrdenTrabajoDto> GetListaOrdenTrabajoEstado(int vigencia);
    }
}
