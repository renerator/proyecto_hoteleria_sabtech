namespace Front_Hoteleria.Dto.Roles
{
    public class RolPermisoDto
    {
        public string Codigo { get; set; }     // ej: "rooms"
        public string Nombre { get; set; }     // ej: "Gestión de Habitaciones"
        public bool Habilitado { get; set; }   // para marcarlo en el modal
    }
}
