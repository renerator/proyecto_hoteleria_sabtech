using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.SolicitudServicio { 
    public class SolicitudKPIModels : EntityBase
    {

        [Key]
        [Column("SolicitudPendientesHoy")]
        public int SolicitudPendientesHoy { get; set; }
        [Column("SolicitudPendientesSemana")]
        public int SolicitudPendientesSemana { get; set; }
        [Column("TiempoPromedioRespuesta")]
        public decimal TiempoPromedioRespuesta { get; set; }  // <-- CAMBIAR A DECIMAL


    }
}
