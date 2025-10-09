using System;

namespace DemoBackend.Dto.Trabajador
{
    public class TrabajadorDto
    {
        public int IdUsuario { get; set; }
        public int IdEmpresaContratista { get; set; }
        public string DNITrabajador { get; set; }
        public string NombresTrabajador { get; set; }
        public string PaternoTrabajador { get; set; }
        public string MaternoTrabajador { get; set; }
        public string EmailTrabajador { get; set; }
        public string CargoTrabajador { get; set; }
        public bool VIP { get; set; }
        public bool EsAdmin { get; set; }
        public bool Estado { get; set; }
    }
}
