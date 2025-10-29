using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.EstadoReserva
{
    // DTO


    // Model (EF/Entity)

    [Table("hot_EstadoReservas]")]
    public class EstadoReservaModels : EntityBase
    {

        [Key]
        [Column("IdEstadoReserva")]
        public int IdEstadoReserva { get; set; }
        [Column("NombreEstadoReserva")]
        public string NombreEstadoReserva { get; set; }
        [Column("Estado")]
        public bool Estado { get; set; }
    }

}
