using Front_Hoteleria.Dto.Habitacion;
using Front_Hoteleria.Dto.Inventario;
using Front_Hoteleria.Dtos.Habitacion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.HabitacionInventario
{
    public interface IHabitacionInventarioService
    {
       
        /// <summary>
        /// Lista el inventario de materiales por habitación.
        /// </summary>
        /// <param name="vigencia">1 = vigente, 0 = inactivo, null = todos (según tu API).</param>
        /// <param name="bearer">Token Bearer para llamar a la API backend.</param>
        /// <returns>Lista de ítems de inventario.</returns>
        Task<List<InventarioHabitacionDTO>> ListarAsync(int vigencia = 1, string bearer = null);

        
    }
}

