using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.ServicioPrioridad
{

    [Table("ctr_man_ServicioPrioridad")]
    public class ServicioPrioridadModels : EntityBase
    {
        [Key]
        [Column("idServicioPrioridad")]
        public int idServicioPrioridad { get; set; }
        [Column("Descripcion")]
        public string Descripcion { get; set; }
    }
}
