using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Bodega
{
    [Table("ctr_man_Bodegas")]
    public class BodegaModels : EntityBase
    {
        [Key]
        [Column("idBodega")]
        public int IdBodega { get; set; }

        [Column("NombreBodega")]
        [StringLength(200)]
        public string? NombreBodega { get; set; }

        [Column("Ubicacion")]
        [StringLength(250)]
        public string? Ubicacion { get; set; }

        [Column("idEmpresa")]
        public int? IdEmpresa { get; set; }

    }
}

