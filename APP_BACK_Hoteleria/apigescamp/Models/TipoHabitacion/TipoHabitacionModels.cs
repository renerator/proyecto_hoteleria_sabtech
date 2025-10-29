using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.TipoHabitacion
{
    public class TipoHabitacionModels : EntityBase
    {
        [Column("IdTipoHabitacion")]
        public int IdTipoHabitacion { get; set; }

        [Column("Descripcion" )]
        public string Descripcion { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; } // 1=Activo, 0=Inactivo


    }
}
