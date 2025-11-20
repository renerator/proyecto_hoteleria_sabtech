using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Huesped
{
    [Table("HOT_HuespedReclamo")]
    public class HuespedReclamoModels : EntityBase
    {
        // Ajusta el nombre de la tabla si la creas con otro nombre
      
        
            [Key]
            [Column("idReclamoHuesped")]
            public int idReclamoHuesped { get; set; }

            [Column("idTipoSolicitudHuesped")]
            public int IdTipoSolicitudHuesped { get; set; }         // FK a hot_TipoSolicitudHuesped

            [Column("TipoSolicitud")]
            public string TipoSolicitud { get; set; }        // texto: reclamo, sugerencia, etc.

            [Column("idCategoriaHuesped")]
            public int IdCategoriaHuesped { get; set; }             // FK a hot_CategoriaSolicitudHuesped

            [Column("Categoria")]
            public string Categoria { get; set; }            // texto: habitación, servicios, etc.

            [Column("Asunto")]
            public string Asunto { get; set; }

            [Column("Descripcion")]
            public string Descripcion { get; set; }

            [Column("Email")]
            public string Email { get; set; }

            [Column("idPrioridad")]
            public int IdPrioridad { get; set; }             // FK a hot_PrioridadHuesped

            [Column("Prioridad")]
            public string Prioridad { get; set; }            // texto: Normal, Alta, Urgente

            [Column("Fecha")]
            public DateTime Fecha { get; set; }

            [Column("idEstado")]
            public int IdEstado { get; set; }                // FK a tabla de estados (si la tienes)

            [Column("Estado")]
            public string Estado { get; set; }               // pendiente, en_revision, respondido, cerrado

            [Column("Respuesta")]
            public string Respuesta { get; set; }

            [Column("FechaRespuesta")]
            public DateTime? FechaRespuesta { get; set; }

            [Column("idUsuarioActualizacion")]
            public int IdUsuarioActualizacion { get; set; }

        }
}