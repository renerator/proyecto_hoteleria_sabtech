using Front_Hoteleria.Model.Habitacion;
using Front_Hoteleria.Models.Habitacion;
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

