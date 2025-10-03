using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Trabajador
{
    [Table("admin_Trabajador")]
    public class TrabajadorModels : EntityBase
    {
        [Key]
        [Column("idTrabajador")]
        public int IdTrabajador { get; set; }

        [Column("idEmpresaContratista")]
        public int IdEmpresaContratista { get; set; }

        [Column("DNITrabajador")]
        [StringLength(20)]
        public string DNITrabajador { get; set; }

        [Column("NombresTrabajador")]
        [StringLength(100)]
        public string NombresTrabajador { get; set; }

        [Column("PaternoTrabajador")]
        [StringLength(50)]
        public string PaternoTrabajador { get; set; }

        [Column("MaternoTrabajador")]
        [StringLength(50)]
        public string MaternoTrabajador { get; set; }

        [Column("EmailTrabajador")]
        [StringLength(100)]
        public string EmailTrabajador { get; set; }

        [Column("CargoTrabajador")]
        [StringLength(100)]
        public string CargoTrabajador { get; set; }

        [Column("VIP")]
        public bool VIP { get; set; }

        [Column("EsAdmin")]
        public bool EsAdmin { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }
    }
}



