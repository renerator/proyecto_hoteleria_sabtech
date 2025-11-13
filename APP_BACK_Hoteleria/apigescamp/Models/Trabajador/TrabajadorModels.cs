using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Trabajador
{
    [Table("admin_Trabajador")]
    public class TrabajadorModels : EntityBase
    {
        [Key]
        [Column("idUsuario")]
        public int IdUsuario { get; set; }

        [Column("idEmpresaContratista")]
        public int IdEmpresaContratista { get; set; }

        [Column("RutTrabajador")]
        [StringLength(20)]
        public string RutTrabajador { get; set; }

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


        [Column("Telefono")]
        [StringLength(10)]
        public string Telefono { get; set; }

        [Column("Observaciones")]
        [StringLength(400)]
        public string Observaciones { get; set; }

        [Column("NivelAcceso")]
        public int NivelAcceso { get; set; }



    }
}



