using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.ServicioEstado
{

    [Table("ctr_man_ServicioEstado")]
    public class ServicioEstadoModels : EntityBase
    {
        [Key]
        [Column("idServicioEstado")]
        public int idServicioEstado { get; set; }
        [Column("Descripcion")]
        public string Descripcion { get; set; }
    }
}
