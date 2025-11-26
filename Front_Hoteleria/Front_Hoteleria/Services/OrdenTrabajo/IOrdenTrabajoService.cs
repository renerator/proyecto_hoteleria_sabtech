using Front_Hoteleria.Dto.OrdenTrabajo;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.OrdenTrabajo{
    public interface IOrdenTrabajoService
    {
        //Task<ReservaKPIDto> ResumenAsync(string bearer = null);

        Task<List<OrdenTrabajoDto>> GetListaOrdenTrabajoEstadoAsync(int vigencia, string bearer = null);

    }
}
