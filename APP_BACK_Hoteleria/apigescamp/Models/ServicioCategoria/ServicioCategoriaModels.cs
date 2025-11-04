using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.ServicioCategoria
{

    [Table("ctr_man_ServiciosCategoria")]
    public class ServicioCategoriaModels :EntityBase
    {


        [Key]
        [Column("idServiciosCategoria")]
        public int IdServiciosCategoria { get; set; }

        [Column("Descripcion")]
        public string Descripcion { get; set; }
    }
}
