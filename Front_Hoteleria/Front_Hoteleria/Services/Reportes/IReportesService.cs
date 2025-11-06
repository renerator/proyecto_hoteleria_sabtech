using System;
using System.Threading.Tasks;
using Front_Hoteleria.Dto.Reportes;

namespace Front_Hoteleria.Services.Reportes
{
    public interface IReportesService
    {
        Task<ReportesKpiDto> ObtenerKpiAsync(string bearer);

        // los 3 métodos que devuelven la misma clase "grande" del resto
        Task<ReportesOperativoDto> GenerarCierreTurnoAsync(DateTime fecha, string turno, string bearer);
        Task<ReportesOperativoDto> GenerarReporteDiarioAsync(DateTime fecha, string bearer);
        Task<ReportesOperativoDto> GenerarAuditoriaAsync(DateTime fechaDesde, DateTime fechaHasta, string bearer);
    }
}
