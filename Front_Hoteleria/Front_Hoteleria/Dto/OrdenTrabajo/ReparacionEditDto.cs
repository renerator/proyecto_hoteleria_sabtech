// Front_Hoteleria/Dto/OrdenTrabajo/ReparacionEditDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Front_Hoteleria.Dto.OrdenTrabajo
{
    public class ReparacionEditDto
    {
        public int IdReparacion { get; set; }
        public string CodigoReparacion { get; set; }

        [Required]
        public int IdHabitacion { get; set; }

        [Required]
        public int IdEstado { get; set; }

        [Required]
        public int IdTipoReparacion { get; set; }

        public int? IdTecnico { get; set; }

        [Required]
        public int IdPrioridad { get; set; }

        public decimal? CostoEstimado { get; set; }

        [Required]
        public string DescripcionProblema { get; set; }

        public string MaterialesNecesarios { get; set; }
        public string NotasAdicionales { get; set; }
    }
}
