using System;

namespace Front_Hoteleria.Dto.SolicitudServicio { 
    public class SolicitudKPIDto
    {
        public int SolicitudPendientesHoy { get; set; }
        public int SolicitudPendientesSemana { get; set; }
        public decimal TiempoPromedioRespuesta { get; set; }  // <-- CAMBIAR A DECIMAL


    }
}
