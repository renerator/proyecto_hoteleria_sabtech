using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Contratos
{
    [Table("hot_Contratos")]
    public class ContratosModels : EntityBase
    {
        [Key]
        [Column("IdContrato")]
        public int IdContrato { get; set; }

        [Column("IdEmpresa")]
        public int? IdEmpresa { get; set; }

        [Column("NumeroContrato")]
        [StringLength(50)]
        public string? NumeroContrato { get; set; }

        [Column("FechaInicio")]
        public DateTime? FechaInicio { get; set; }

        [Column("FechaFin")]
        public DateTime? FechaFin { get; set; }

        [Column("Valor")]
        public decimal? Valor { get; set; }

        [Column("IdCampamento")]
        public int? IdCampamento { get; set; }

        [Column("MaximoTrabajadores")]
        public int? MaximoTrabajadores { get; set; }

        [Column("Descripcion")]
        public string? Descripcion { get; set; }

        // NUEVO: FK al catálogo de tipos de contrato
        [Column("IdTipoContrato")]
        public int? IdTipoContrato { get; set; }

        // ahora es bit (1 activo, 0 no activo)
        [Column("Estado")]
        public bool Estado { get; set; }
    }
}
