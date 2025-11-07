namespace Front_Hoteleria.Dto.Roles
{
    public class TrabajadorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Empresa { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
    }
}
