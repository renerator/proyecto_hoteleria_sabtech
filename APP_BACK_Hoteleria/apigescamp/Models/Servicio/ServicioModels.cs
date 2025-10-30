using DemoBackend.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Servicio
{
    [Table("ctr_man_Servicios")]
    public class ServicioModels : EntityBase
    {
        [Key]
        [Column("idServicio")]
        public int IdServicio { get; set; }

        [Column("NombreServicio")]
        [StringLength(150)]
        public string NombreServicio { get; set; }

        [Column("idTipoServicio")]
        public int IdTipoServicio { get; set; }

        [Column("idEmpresa")]
        public int IdEmpresa { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }

        // Nuevos campos
        [Column("idServicioPrioridad")]
        public int? IdServicioPrioridad { get; set; }

        [Column("idServiciosCategoria")]
        public int? IdServiciosCategoria { get; set; }

        // OJO: el nombre en BD está con doble 't': TiempoEsttimado
        [Column("TiempoEsttimado")]
        public int TiempoEstimadoMinutos { get; set; }

        [Column("Precio")]
        public int? Precio { get; set; }

        // Helper opcional para mostrar HH:mm en la UI
        [NotMapped]
        public string TiempoEstimadoFmt =>
            TimeSpan.FromMinutes(TiempoEstimadoMinutos).ToString(@"hh\:mm");

        // (Opcional) Navegación si tienes los modelos creados:
        // [ForeignKey(nameof(IdServicioPrioridad))]
        // public virtual ServicioPrioridadModels ServicioPrioridad { get; set; }
        //
        // [ForeignKey(nameof(IdServiciosCategoria))]
        // public virtual ServiciosCategoriaModels ServiciosCategoria { get; set; }
    }
}



