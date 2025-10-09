// Models/Auditoria/AuditoriaModel.cs
using System;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Auditoria
{
    [Table("AUDIT_LOG")]
    public class AuditoriaModel : EntityBase
    {
        // Si EntityBase exige 'Id', lo mapeamos a IdAuditoria:
        [Key]
        [Column("IdAuditoria")]
        public int Id { get; set; }  // PK (mapeado a IdAuditoria)

        [Column("idUsuario")]
        public int? IdUsuario { get; set; }

        [Column("Accion")]
        [StringLength(50)]
        public string Accion { get; set; } = string.Empty;

        [Column("Modulo")]
        [StringLength(250)]
        public string Modulo { get; set; } = string.Empty;

        [Column("FechaAccion")]
        public DateTime FechaAccion { get; set; }

        [Column("TablaAfectada")]
        [StringLength(100)]
        public string TablaAfectada { get; set; } = string.Empty;
    }
}
