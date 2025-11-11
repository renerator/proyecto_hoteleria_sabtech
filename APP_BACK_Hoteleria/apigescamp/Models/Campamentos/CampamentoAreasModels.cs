using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Campamentos
{
    [Table("hot_CampamentoAreas")]
    public class CampamentoAreasModels : EntityBase
    {
        [Key]
        [Column("IdCampamentoArea")]
        public int IdCampamentoArea { get; set; }
        [Column("IdCampamento")]
        public int IdCampamento { get; set; }
        [Column("Nombre")]
        [StringLength(200)]
        public string? Nombre { get; set; }
        [Column("Tipo")]
        [StringLength(50)]
        public string? Tipo { get; set; }
        [Column("Capacidad")]
        public int? Capacidad { get; set; }
        [Column("Estado")]
        [StringLength(20)]
        public string? Estado { get; set; }
    }
}
