using DemoBackend.Models;

namespace DemoBackend.Dto
{
    public class LoginDto
    {

        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int idUsuario { get; set; }
        public int idPerfil { get; set; }


    }
}
