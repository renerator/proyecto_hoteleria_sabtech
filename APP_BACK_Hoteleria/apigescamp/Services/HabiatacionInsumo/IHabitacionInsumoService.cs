using System.Collections.Generic;
using DemoBackend.Dto.HabitacionInsumo;

namespace DemoBackend.Services.HabitacionInsumo
{
    public interface IHabitacionInsumoService
    {
        public List<HabitacionInsumoDto> GetListaHabitacionInsumoEstado(int Vigente);
        bool CrearHabitacionInsumo(HabitacionInsumoDto dto);
        bool ModificarHabitacionInsumo(HabitacionInsumoDto dto);
        bool EliminarHabitacionInsumo(int idHabitacionInsumo);
        
    }
}