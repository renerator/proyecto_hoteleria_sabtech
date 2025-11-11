using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Campamentos
{
    [Table("hot_Campamentos")]
    public class CampamentosModels : EntityBase
    {
        [Key]
        [Column("IdCampamento")]
        public int IdCampamento { get; set; }
        [Column("Nombre")]
        [StringLength(200)]
        public string? Nombre { get; set; }
        [Column("Codigo")]
        [StringLength(50)]
        public string? Codigo { get; set; }
        [Column("Ubicacion")]
        [StringLength(200)]
        public string? Ubicacion { get; set; }
        [Column("Capacidad")]
        public int? Capacidad { get; set; }
        [Column("OcupacionActual")]
        public int? OcupacionActual { get; set; }
        [Column("Estado")]
        [StringLength(20)]
        public string? Estado { get; set; }
        [Column("Encargado")]
        [StringLength(200)]
        public string? Encargado { get; set; }
        [Column("Descripcion")]
        public string? Descripcion { get; set; }
    }
}
