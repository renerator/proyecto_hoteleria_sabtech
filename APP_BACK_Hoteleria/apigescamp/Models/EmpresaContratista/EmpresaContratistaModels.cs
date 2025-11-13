// DemoBackend/Models/EmpresaContratista/EmpresaContratistaModels.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.EmpresaContratista
{
    [Table("admin_EmpresaContratista", Schema = "dbo")]
    public class EmpresaContratistaModels:EntityBase
    {
        [Key]
        [Column("idEmpresaContratista")]
        public int idEmpresaContratista { get; set; }

        [Column("NombreEmpresaContratista")]
        [StringLength(200)] // ajusta si tu columna tiene otro tamaño
        public string? NombreEmpresaContratista { get; set; }

        [Column("DNIEmpresaContratista")]
        [StringLength(50)]
        public string? DNIEmpresaContratista { get; set; }

        [Column("DireccionEmpresaContratista")]
        [StringLength(250)]
        public string? DireccionEmpresaContratista { get; set; }

        [Column("idPais")]
        public int? idPais { get; set; }

        [Column("idEmpresa")]
        public int? idEmpresa { get; set; }

        // Si la columna es BIT usa bool?, si es VARCHAR(1) cambia a string? (p.ej. "A"/"I" o "1"/"0")
        [Column("Estado")]
        public bool? Estado { get; set; }

        [Column("TelefonoEmpresa")]
        [StringLength(30)]
        public string? TelefonoEmpresa { get; set; }

        [Column("EmailEmpresa")]
        [StringLength(150)]
        public string? EmailEmpresa { get; set; }

        [Column("ContactoPrincipal")]
        [StringLength(150)]
        public string? ContactoPrincipal { get; set; }

        [Column("DescripcionEmpresa")]
        [StringLength(500)]
        public string? DescripcionEmpresa { get; set; }
    }
}
