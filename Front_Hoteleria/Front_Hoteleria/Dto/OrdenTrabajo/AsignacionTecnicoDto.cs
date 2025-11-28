// Front_Hoteleria/Dto/OrdenTrabajo/AsignacionTecnicoDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Front_Hoteleria.Dto.OrdenTrabajo
{
    public class AsignacionTecnicoDto
    {
        public int IdReparacion { get; set; }

        public string CodigoReparacion { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un técnico.")]
        public int? IdTecnico { get; set; }

        [Required(ErrorMessage = "Debe indicar la fecha de asignación.")]
        [DataType(DataType.Date)]
        public DateTime? FechaAsignacion { get; set; }

        [Required(ErrorMessage = "Debe indicar la hora estimada de inicio.")]
        [DataType(DataType.Time)]
        public string HoraInicio { get; set; }

        [Required(ErrorMessage = "Debe indicar el tiempo estimado.")]
        [Range(0, 1000, ErrorMessage = "El tiempo estimado debe ser mayor o igual a 0.")]
        public decimal? TiempoEstimadoHoras { get; set; }

        public string ComentariosAsignacion { get; set; }
    }
}
