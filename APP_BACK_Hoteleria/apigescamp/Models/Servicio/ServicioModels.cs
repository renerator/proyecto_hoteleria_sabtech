using DemoBackend.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Servicio
{
    [Table("ctr_man_Servicios")]
    public class ServicioModels : EntityBase
    {
        [Key]
        [Column("idServicio")]
        public int IdServicio { get; set; }

        [Column("NombreServicio")]
        [StringLength(150)]
        public string NombreServicio { get; set; }

        [Column("idTipoServicio")]
        public int IdTipoServicio { get; set; }

        [Column("idEmpresa")]
        public int IdEmpresa { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }
    }
}


