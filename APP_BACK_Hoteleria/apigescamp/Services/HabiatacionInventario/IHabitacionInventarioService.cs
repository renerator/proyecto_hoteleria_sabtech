using System.Collections.Generic;
using DemoBackend.Dto.HabitacionInventario;

namespace DemoBackend.Services.HabitacionInventario
{
    public interface IHabitacionInventarioService
    {//cambio 1-12
        public List<HabitacionInventarioDto> GetListaHabitacionInsumoEstado(int Vigente);
        bool CrearHabitacionInsumo(HabitacionInventarioDto dto);
        bool ModificarHabitacionInsumo(HabitacionInventarioDto dto);
        bool EliminarHabitacionInsumo(int idHabitacionInsumo);
        
    }
}