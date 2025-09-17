using DemoBackend.Models;

namespace DemoBackend.Configurations
{
    public class CredencialesConfig : EntityBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Conexion { get; set; }
    }
}