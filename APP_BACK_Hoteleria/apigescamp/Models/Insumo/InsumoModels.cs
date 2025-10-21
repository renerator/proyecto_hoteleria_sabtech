using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Insumos
{
    [Table("ctr_man_Insumos")]
    public class InsumoModels : EntityBase
    {
        [Key]
        [Column("idInsumo")]
        public int IdInsumo { get; set; }

        [Column("NombreInsumo")]
        [StringLength(200)]
        public string? NombreInsumo { get; set; }

        [Column("StockMinimo")]
        public int? StockMinimo { get; set; }

        [Column("idBodega")]
        public int? IdBodega { get; set; }

       
    }
}
