using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models
{
    public class Login : EntityBase
    {

        
        [Column("idCredencial")]
        public int id { get; set; }

        [Column("username")]
        public string username { get; set; }

        [Column("password")]
        public string password { get; set; }

        [Column("IdUsuario")]
        public int idUsuario { get; set; }
        [Column("idPerfil")]
        public int idPerfil { get; set; }
    }
}
