using DemoBackend.Models;
using System;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DemoBackend.Models.BitacoraHabitacion
{
    public class BitacoraHabitacionModels : EntityBase
    {
        public int IdBitacoraHabitacion { get; set; }
        public int IdHabitacion { get; set; }
        public DateTime? FechaBitacora { get; set; }
        public string? TipoBitacora { get; set; }



    }
}


