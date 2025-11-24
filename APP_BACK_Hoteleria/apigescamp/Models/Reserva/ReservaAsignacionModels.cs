using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Reserva
{
    [Table("hot_ReservaAsignacion")]
    public class ReservaAsignacionModels : EntityBase
    {
        [Key]
        [Column("IdReservaAsignacion")]
        public int IdReservaAsignacion { get; set; }

        [Column("IdReserva")]
        public int IdReserva { get; set; }

        [Column("IdHabitacion")]
        public int IdHabitacion { get; set; }

        [Column("IdEmpresa")]
        public int? IdEmpresa { get; set; }

        [Column("IdTipoEmpresa")]
        public int? IdTipoEmpresa { get; set; }

        [Column("IdJornada")]
        public int? IdJornada { get; set; }

        [Column("IdHorario")]
        public int? IdHorario { get; set; }

        [Column("IdGenero")]
        public int? IdGenero { get; set; }

        [Column("CantidadSupervisores")]
        public int? CantidadSupervisores { get; set; }

        [Column("CantidadTrabajadores")]
        public int? CantidadTrabajadores { get; set; }

        [Column("Observaciones")]
        public string Observaciones { get; set; }
    }
}
