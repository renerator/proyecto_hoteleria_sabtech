using System;
using System.Collections.Generic;

namespace DemoBackend.Dto.Contratos
{
    public class ContratoDto
    {//cambio 1-12
        public int IdContrato { get; set; }
        public int? IdEmpresa { get; set; }
        public string? Empresa { get; set; }
        public string? RutEmpresa { get; set; }
        public string? NumeroContrato { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal? Valor { get; set; }
        public int? IdCampamento { get; set; }
        public string? Campamento { get; set; }
        public int? MaximoTrabajadores { get; set; }
        public string? Descripcion { get; set; }

        public int? IdTipoContrato { get; set; }
        public string TipoContratoNombre { get; set; }
        public bool? Estado { get; set; }
        public List<ContratoTrabajadorDto> Trabajadores { get; set; } = new();
    }

    public class ContratoTrabajadorDto
    {
        public int Id { get; set; }
        public int IdContrato { get; set; }
        public int? IdTrabajador { get; set; }
        public string? Nombre { get; set; }
        public string? Rut { get; set; }
        public string? Cargo { get; set; }
        public string? Estado { get; set; }
    }

    public class ContratoKpiDto
    {
        public int ContratosActivos { get; set; }
        public int EmpresasRegistradas { get; set; }
        public int TrabajadoresActivos { get; set; }
        public int VencenPronto { get; set; }
    }
}
