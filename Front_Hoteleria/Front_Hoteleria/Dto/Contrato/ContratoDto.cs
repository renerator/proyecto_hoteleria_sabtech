using System;
using System.Collections.Generic;

namespace Front_Hoteleria.Dto.Contrato
{
    public class ContratoDto
    {
        public int IdContrato { get; set; }
        public int? IdEmpresa { get; set; }
        public string Empresa { get; set; }
        public string RutEmpresa { get; set; }

        public string NumeroContrato { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public string Tipo { get; set; }          // temporal, indefinido, proyecto, servicio
        public decimal? Valor { get; set; }

        public int? IdCampamento { get; set; }
        public string Campamento { get; set; }

        public int? MaximoTrabajadores { get; set; }
        public string Descripcion { get; set; }

        public int? IdTipoContrato { get; set; }
        public string TipoContratoNombre { get; set; }
        public bool Estado { get; set; }        // Activo, Vencido, Pendiente, Suspendido

        public List<ContratoTrabajadorDto> Trabajadores { get; set; } = new List<ContratoTrabajadorDto>();
    
    }
}
