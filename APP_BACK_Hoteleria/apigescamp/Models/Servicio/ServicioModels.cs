using DemoBackend.Models;
using DemoBackend.Models.ServicioCategoria;
using DemoBackend.Models.ServicioEstado;
using DemoBackend.Models.ServicioPrioridad;
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
        public string NombreServicio { get; set; }

        [Column("idTipoServicio")]
        public int IdTipoServicio { get; set; }

        [Column("idEmpresa")]
        public int IdEmpresa { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }

        [Column("idServicioEstado")]
        public int? IdServicioEstado { get; set; }

        [Column("idServicioPrioridad")]
        public int? IdServicioPrioridad { get; set; }

        [Column("idServiciosCategoria")]
        public int? IdServiciosCategoria { get; set; }

        [Column("TiempoEsttimado")]
        public int TiempoEstimadoMinutos { get; set; }

        [Column("Precio")]
        public int? Precio { get; set; }

        // ====== NUEVAS columnas que vienen del SP (no están en la tabla física) ======
       // [NotMapped]
        public string NombreEstadoServicio { get; set; }

       // [NotMapped]
        public string NombreCategoria { get; set; }

       // [NotMapped]
        public string NombrePrioridad { get; set; }

        // si quieres seguir teniendo las navegaciones, las dejas:
        [ForeignKey(nameof(IdServicioEstado))]
        public virtual ServicioEstadoModels ServicioEstado { get; set; }

        [ForeignKey(nameof(IdServicioPrioridad))]
        public virtual ServicioPrioridadModels ServicioPrioridad { get; set; }

        [ForeignKey(nameof(IdServiciosCategoria))]
        public virtual ServicioCategoriaModels ServiciosCategoria { get; set; }
    }
}




