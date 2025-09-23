using DemoBackend.Dto.Habitacion;
using System.Collections.Generic;

namespace DemoBackend.Services.Habitacion
{
    public interface IHabitacionService
    {
      

        List<HabitacionDto> GetListaHabitacion();
        List<HabitacionDto> GetListaHabitacionEstado(int estado);
        public bool CrearHabitacion(HabitacionDto HabitacionModels);
        public bool ModificarHabitacion(HabitacionDto HabitacionModels);
        public bool EliminarHabitacion(HabitacionDto HabitacionModels);
        List<HabitacionDto> VerificaHabitacionPorNombre(HabitacionDto HabitacionModels);

        List<HabitacionDto> VerificaHabitacionPorId(HabitacionDto HabitacionModels);
    }
}
