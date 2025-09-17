using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Grupos
{
    public class GruposModels : EntityBase
    {
        [Key]
        [Column("idGrupos")]
        public int idGrupos { get; set; }
        [Column("NombreGrupo")]
        public string NombreGrupo { get; set; }
        [Column("Descripcion")]
        public string Descripcion { get; set; }
        [Column("Visualiza")]
        public bool Visualiza { get; set; }
        [Column("Edita")]
        public bool Edita { get; set; }
        [Column("Agrega")]
        public bool Agrega { get; set; }
        [Column("Elimina")]
        public bool Elimina { get; set; }
        [Column("EstadoRegistro")]
        public bool EstadoRegistro { get; set; }
        [Column("EstadoTodos")]
        public bool EstadoTodos { get; set; }
        [Column("FechaCreacion")]
        public DateTime? FechaCreacion { get; set; }
        [Column("FechaActualizacion")]
        public DateTime? Fechaactualizacion { get; set; }
        [Column("IdUsuarioCreacion")]
        public int IdUsuarioCreacion { get; set; }
        [Column("idUsuarioActualizacion")]
        public int IdUsuarioActualizacion { get; set; }
    }
}
