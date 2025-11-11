using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Reserva
{
    public class CampamentoKPIModels : EntityBase
    {
        public int CampamentosActivos { get; set; }
        public int AreasComunes { get; set; }
        public int Habitaciones { get; set; }
        public decimal TasaUtilizacion { get; set; }
    }
}


