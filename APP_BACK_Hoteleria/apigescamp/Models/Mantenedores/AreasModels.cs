using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Mantenedores
{
    public class AreasModels : EntityBase
    {
        [Key]
        [Column("idArea")]
        public int idArea { get; set; }
        [Column("NombreArea")]
        public string NombreArea { get; set; }

        [Column("idEmpresa")]
        public int idEmpresa { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }
        // [Column("AcronimoArea")]
        // public string AcronimoArea { get; set; }

    }
}
