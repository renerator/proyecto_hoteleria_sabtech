using DemoBackend.Models;
using System;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Menu
{
    public class MenuModels : EntityBase
    {
        [Key]
        [Column("idFuncionalidad")]
        public int IdFuncionalidad { get; set; }

        [Column("descripcion")]
        public string Controller { get; set; }

        [Column("Pagina")]
        public string Pagina { get; set; }

        [Column("Menu")]
        public string Menu { get; set; }
    }
}


