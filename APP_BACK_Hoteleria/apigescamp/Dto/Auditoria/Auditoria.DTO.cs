using System;


    // Dto/Auditoria/AuditoriaDto.cs
    namespace DemoBackend.Dto.Auditoria
    {
        public class AuditoriaDto
        {
            public int? IdUsuario { get; set; }
            public string Accion { get; set; } = string.Empty;
            public string Modulo { get; set; } = string.Empty;
            public DateTime FechaAccion { get; set; }
            public string TablaAfectada { get; set; } = string.Empty;
        }
    }
