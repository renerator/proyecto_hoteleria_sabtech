using System;

namespace DemoBackend.Dto.SolicitudServicio { 
    public class SolicitudKPIDto
    {
        public int SolicitudPendientesHoy { get; set; }
        public int SolicitudPendientesSemana { get; set; }
        public decimal TiempoPromedioRespuesta { get; set; }   // <-- decimal también


    }
}
