namespace Front_Hoteleria.Dto.Dotaciones
{
    public class DotacionDto
    {
        public int IdDotacion { get; set; }

        // empresa / contrato
        public int? IdEmpresa { get; set; }
        public string Empresa { get; set; }
        public bool Estado { get; set; }
        // persona
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Rut { get; set; }

        // datos laborales
        public string Cargo { get; set; }

        /// <summary>
        /// Valores esperados: "Día", "Noche", "Mantenimiento", "Fuera"
        /// </summary>
        public string Turno { get; set; }

        // contacto
        public string Telefono { get; set; }
        public string Email { get; set; }

        // otros
        public string Observaciones { get; set; }

        // helper para la vista
        public string NombreCompleto
        {
            get
            {
                var n = (Nombre ?? "").Trim();
                var a = (Apellido ?? "").Trim();
                return string.IsNullOrEmpty(a) ? n : (n + " " + a);
            }
        }
    }
}
