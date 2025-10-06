using System;

namespace DemoBackend.Dto.Servicio
{
    public class ServicioDto
    {
        public int IdServicio { get; set; }        // Identificador único del servicio
        public string NombreServicio { get; set; } // Ej: Limpieza, Reparación, etc.
        public int IdTipoServicio { get; set; }    // Tipo de servicio (relación a otra tabla)
        public int IdEmpresa { get; set; }         // Empresa asociada
        public bool Estado { get; set; }           // true = Activo, false = Inactivo o eliminado
    }
}
