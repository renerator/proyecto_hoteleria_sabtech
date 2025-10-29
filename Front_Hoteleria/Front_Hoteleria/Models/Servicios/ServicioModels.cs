using System;

namespace Front_Hoteleria.Dtos
{
    public class ServicioDtos
    {

        public int IdServicio { get; set; }
        public string NombreServicio { get; set; }
        public int IdTipoServicio { get; set; }
        public int IdEmpresa { get; set; }         // Empresa asociada
        public bool Estado { get; set; }           // true = Activo, false = Inactivo o eliminado
    }
}
