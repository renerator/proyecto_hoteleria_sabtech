namespace Front_Hoteleria.Dto.Contrato
{
    public class ContratoTrabajadorDto
    {
        public int IdTrabajador { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Rut { get; set; }
        public string Cargo { get; set; }
        public string NivelAcceso { get; set; }   // admin, manager, worker, guest
        public string Telefono { get; set; }
        public string Email { get; set; }

        public string NombreCompleto =>
            ((Nombres ?? "").Trim() + " " + (Apellidos ?? "").Trim()).Trim();
    }
}
