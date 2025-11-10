// Dto/Reclamos/ReclamoSolicitudDto.cs
using System;

namespace Front_Hoteleria.Dto.Huesped
{
    public class ReclamoSolicitudDto
    {
        public string Id { get; set; }              // sug_...
        public string Tipo { get; set; }            // sugerencia, reclamo, felicitacion, queja
        public string Categoria { get; set; }       // habitacion, servicios, comida...
        public string Asunto { get; set; }
        public string Descripcion { get; set; }
        public string Email { get; set; }
        public string Prioridad { get; set; }       // normal, alta, urgente
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }          // pendiente, en_revision, respondido, cerrado
        public string Respuesta { get; set; }
        public DateTime? FechaRespuesta { get; set; }
    }
}
