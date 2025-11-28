// Front_Hoteleria/Dto/Habitacion/AuditoriaInventarioDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Front_Hoteleria.Dto.Habitacion
{
    public class AuditoriaInventarioDto
    {
        public int IdInventario { get; set; }

        public string CodigoInventario { get; set; }     // INV-0001, etc.

        [Required]
        public int? IdEstadoEncontrado { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? FechaAuditoria { get; set; }

        [Required]
        public TimeSpan? HoraAuditoria { get; set; }

        [Required]
        public int? IdAuditor { get; set; }

        [Required]
        [StringLength(1000)]
        public string Observaciones { get; set; }

        public bool TieneFotografias { get; set; }
        public bool RequiereAccionCorrectiva { get; set; }
    }
}
