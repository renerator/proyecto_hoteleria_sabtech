using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Empresa
{
    [Table("admin_EmpresaContratista")]
    public class EmpresaModels : EntityBase
    {
        [Key]
        [Column("idEmpresaContratista")]
        public int IdEmpresaContratista { get; set; }

        [Column("NombreEmpresaContratista")]
        public string? NombreEmpresaContratista { get; set; }

        [Column("DNIEmpresaContratista")]
        public string? DNIEmpresaContratista { get; set; }   // RUT

        [Column("DireccionEmpresaContratista")]
        public string? DireccionEmpresaContratista { get; set; }

        [Column("idPais")]
        public int? IdPais { get; set; }

        [Column("idEmpresa")]
        public int? IdEmpresa { get; set; }                  // FK que usarás en el combo

        [Column("Estado")]
        public bool Estado { get; set; }                     // bit (1 activo, 0 inactivo)

        [Column("TelefonoEmpresa")]
        public string? TelefonoEmpresa { get; set; }

        [Column("EmailEmpresa")]
        public string? EmailEmpresa { get; set; }

        [Column("ContactoPrincipal")]
        public string? ContactoPrincipal { get; set; }

        [Column("DescripcionEmpresa")]
        public string? DescripcionEmpresa { get; set; }
    }
}
