using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Reserva
{
    public class ReservaEstadoModels : EntityBase
    {
        public int IdEstadoReserva { get; set; }
        public string NombreEstado { get; set; } = "";
        public int Cantidad { get; set; }
    }
}


