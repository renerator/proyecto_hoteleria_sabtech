using System.Collections.Generic;

namespace Front_Hoteleria.Dto.Habitacion
{
    /// <summary>
    /// Resultado global de la carga masiva de habitaciones.
    /// </summary>
    public class CargaMasivaHabitacionesResultadoDto
    {
        public CargaMasivaHabitacionesResultadoDto()
        {
            Detalle = new List<CargaMasivaHabitacionFilaDto>();
        }

        /// <summary>
        /// Total de filas leídas desde el archivo.
        /// </summary>
        public int TotalFilas { get; set; }

        /// <summary>
        /// Cantidad de filas válidas (insertadas/actualizadas).
        /// </summary>
        public int FilasCorrectas { get; set; }

        /// <summary>
        /// Cantidad de filas con error.
        /// </summary>
        public int FilasConError { get; set; }

        /// <summary>
        /// Detalle por fila (útil para mostrar un log o exportar errores).
        /// </summary>
        public List<CargaMasivaHabitacionFilaDto> Detalle { get; set; }
    }
}
