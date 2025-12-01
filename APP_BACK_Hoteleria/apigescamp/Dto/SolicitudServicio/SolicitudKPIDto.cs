using System;

namespace DemoBackend.Dto.SolicitudServicio { 
    public class SolicitudKPIDto
    {//cambio 1-12
        public int SolicitudPendientesHoy { get; set; }
        public int SolicitudPendientesSemana { get; set; }
        public decimal TiempoPromedioRespuesta { get; set; }   // <-- decimal también


    }
}
