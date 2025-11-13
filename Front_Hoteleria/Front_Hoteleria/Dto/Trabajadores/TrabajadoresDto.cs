using System;

namespace Font_Hoteleria.Dto.Trabajadores
{
    public class TrabajadoresDto
    {
        public int IdUsuario { get; set; }
        public int IdEmpresaContratista { get; set; }
        public string RutTrabajador { get; set; }
        public string NombresTrabajador { get; set; }
        public string PaternoTrabajador { get; set; }
        public string MaternoTrabajador { get; set; }
        public string EmailTrabajador { get; set; }
        public string CargoTrabajador { get; set; }
        public bool VIP { get; set; }
        public bool EsAdmin { get; set; }
        public bool Estado { get; set; }
        public string Telefono { get; set; }
        public int NivelAcceso { get; set; }
        public string Observaciones { get; set; }



    }
}
