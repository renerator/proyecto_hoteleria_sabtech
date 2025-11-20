// Dto/Reclamos/ReclamoSolicitudDto.cs
using System;

namespace DemoBackend.Dto.Huesped
{
    public class ReclamoSolicitudDto
    {
        public int idReclamoHuesped { get; set; }

        public int IdTipoSolicitudHuesped { get; set; } // sug_...
        public string TipoSolicitud { get; set; }            // sugerencia, reclamo, felicitacion, queja
        public int IdCategoriaHuesped { get; set; } // sug_...
        public string Categoria { get; set; }       // habitacion, servicios, comida...
        public string Asunto { get; set; }
        public string Descripcion { get; set; }
        public string Email { get; set; }

        public int IdPrioridad { get; set; }
        public string Prioridad { get; set; }       // normal, alta, urgente
        public DateTime Fecha { get; set; }

        public int IdEstado{ get; set; }
        public string Estado { get; set; }          // pendiente, en_revision, respondido, cerrado
        public string Respuesta { get; set; }
        public DateTime? FechaRespuesta { get; set; }

        public int IdUsuarioActualizacion { get; set; }
    }
}
