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
        [Column("idTipoFuncionalidad")]
        public int IdTipoFuncionalidad { get; set; }
       

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [Column("Pagina")]
        public string Pagina { get; set; }

        [Column("EsMenu")]
        public int EsMenu { get; set; }
    }
}


