using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Habitacion
{
    [Table("ctr_man_habitaciones")]
    public class HabitacionModels : EntityBase
    {
        [Key]
        [Column("idHabitacion")]
        public int IdHabitacion { get; set; }

        [Column("idArea")]
        public int IdArea { get; set; }

        [Column("NombreHabitacion")]
        public string NombreHabitacion { get; set; }

        [Column("Capacidad")]
        public int Capacidad { get; set; }

        [Column("VIP")]
        public bool VIP { get; set; }

        [Column("idEstado")]
        public int IdEstado { get; set; }

        [Column("idEmpresa")]
        public int IdEmpresa { get; set; }

        [Column("Motivo")]
        public string Motivo { get; set; }
    }
}

