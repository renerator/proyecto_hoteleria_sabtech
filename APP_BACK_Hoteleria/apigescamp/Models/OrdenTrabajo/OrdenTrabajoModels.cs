using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;





namespace DemoBackend.Models.OrdenTrabajo { 
    
        [Table("hot_OrdenesTrabajo")]
        public class OrdenTrabajoModels : EntityBase
        {
            [Key]
            [Column("idOrdenTrabajo")]
            public int IdOrdenTrabajo { get; set; }

            [Column("NumeroOT")]
            [StringLength(50)]
            public string NumeroOT { get; set; }

            [Column("FechaIngresoOT")]
            public DateTime FechaIngresoOT { get; set; }

            [Column("FechaCierreOT")]
            public DateTime? FechaCierreOT { get; set; }

            [Column("idHabitacion")]
            public int IdHabitacion { get; set; }

            [Column("Estado")]
            [StringLength(50)]
            public string Estado { get; set; }

            [Column("idTipo")]
            public int? IdTipo { get; set; }

        [Column("Tecnico")]
        public string Tecnico { get; set; }

        [Column("NombreTipo")]
            [StringLength(100)]
        public string NombreTipo { get; set; }
        [Column("Descripcion")]
            [StringLength(500)]
            public string Descripcion { get; set; }

            [Column("idPrioridad")]
            public int? IdPrioridad { get; set; }

        [Column("Prioridad")]
        public string Prioridad { get; set; }

        [Column("idEstado")]
            public int? IdEstado { get; set; }

            [Column("idTecnico")]
            public int? IdTecnico { get; set; }

            [Column("TiempoMinutos")]
            public int? TiempoMinutos { get; set; }

            [Column("FechaActualizacion")]
            public DateTime? FechaActualizacion { get; set; }

            [Column("IdUsuario")]
            public int? IdUsuario { get; set; }
        }
    }
