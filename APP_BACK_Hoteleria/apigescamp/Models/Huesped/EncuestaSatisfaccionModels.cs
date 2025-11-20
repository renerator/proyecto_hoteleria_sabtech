using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Huesped
{
    [Table("hot_EncuestaSatisfaccion")]
    public class EncuestaSatisfaccionModels : EntityBase
    {
        [Key]
        [Column("IdEncuesta")]
        public int IdEncuesta { get; set; }

        [Column("IdReserva")]
        public int? IdReserva { get; set; }

        /// <summary>
        /// 1 = Check-in, 2 = Check-out
        /// </summary>
        [Column("TipoEncuesta")]
        public int TipoEncuesta { get; set; }

        [Column("CalificacionGeneral")]
        public int? CalificacionGeneral { get; set; }

        [Column("AtencionPersonal")]
        public int? AtencionPersonal { get; set; }

        [Column("LimpiezaHabitacion")]
        public int? LimpiezaHabitacion { get; set; }

        [Column("FacilidadesHotel")]
        public int? FacilidadesHotel { get; set; }

        [Column("RelacionCalidadPrecio")]
        public int? RelacionCalidadPrecio { get; set; }

        [Column("Comentarios")]
        public string Comentarios { get; set; }

        /// <summary>
        /// 1 = Sí, 2 = Probablemente, 3 = No
        /// </summary>
        [Column("Recomendaria")]
        public int? Recomendaria { get; set; }
    }
}
