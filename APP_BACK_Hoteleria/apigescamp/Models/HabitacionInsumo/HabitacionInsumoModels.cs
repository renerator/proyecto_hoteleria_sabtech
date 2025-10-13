using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.HabitacionInsumo
{
    [Table("ctr_man_HabitacionInsumos")]
    public class HabitacionInsumoModels : EntityBase
    {
        [Key]
        [Column("idHabitacionInsumo")]
        public int IdHabitacionInsumo { get; set; }

        [Column("idHabitacion")]
        public int IdHabitacion { get; set; }

        [Column("idInsumo")]
        public int IdInsumo { get; set; }

    }
}