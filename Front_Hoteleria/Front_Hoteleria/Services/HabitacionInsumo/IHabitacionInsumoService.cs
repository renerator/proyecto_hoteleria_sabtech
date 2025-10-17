using Front_Hoteleria.Dto.Habitacion;
using Front_Hoteleria.ViewModels.Habitacion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.HabitacionInsumo
{
    public interface IHabitacionInsumoService
    {
        Task<List<InventarioFilaVm>> ListarAsync(int vigencia = 1, string bearer = null);
    }
}

