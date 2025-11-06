using Front_Hoteleria.Dto.Reportes;
using Front_Hoteleria.Services.Api;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Reportes
{
    public class ReportesService : IReportesService
    {
        private static readonly HttpClient _http;

        static ReportesService()
        {
            _http = new HttpClient();

            // misma lógica que tu CheckinService
            var baseUrl = ConfigurationManager.AppSettings["Api.BaseUrl"]
                       ?? ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (!string.IsNullOrWhiteSpace(baseUrl))
                _http.BaseAddress = new Uri(baseUrl);
        }

        private void SetBearer(string bearer)
        {
            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(bearer))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        // =========================
        // 1) KPI / DASHBOARD
        // =========================
        public async Task<ReportesKpiDto> ObtenerKpiAsync(string bearer = null)
        {
            SetBearer(bearer);

            // ajusta al endpoint real de tu API
            var res = await _http.GetAsync("/api/Reportes/Dashboard");
            if (!res.IsSuccessStatusCode)
                return new ReportesKpiDto();

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReportesKpiDto>(json) ?? new ReportesKpiDto();
        }

        // =========================
        // 2) CIERRE DE TURNO
        // =========================
        public async Task<ReportesOperativoDto> GenerarCierreTurnoAsync(DateTime fecha, string turno, string bearer = null)
        {
            SetBearer(bearer);

            var payload = new
            {
                fecha = fecha.ToString("yyyy-MM-dd"),
                turno = turno
            };

            var jsonBody = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // ajusta el endpoint si tu API se llama distinto
            var res = await _http.PostAsync("/api/Reportes/CierreTurno", content);
            if (!res.IsSuccessStatusCode)
                return null;

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReportesOperativoDto>(json);
        }

        // =========================
        // 3) REPORTE DIARIO
        // =========================
        public async Task<ReportesOperativoDto> GenerarReporteDiarioAsync(DateTime fecha, string bearer = null)
        {
            SetBearer(bearer);

            var payload = new
            {
                fecha = fecha.ToString("yyyy-MM-dd")
            };

            var jsonBody = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var res = await _http.PostAsync("/api/Reportes/ReporteDiario", content);
            if (!res.IsSuccessStatusCode)
                return null;

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReportesOperativoDto>(json);
        }

        // =========================
        // 4) AUDITORÍA OCUPACIÓN
        // =========================
        public async Task<ReportesOperativoDto> GenerarAuditoriaAsync(DateTime fechaDesde, DateTime fechaHasta, string bearer = null)
        {
            SetBearer(bearer);

            var payload = new
            {
                fechaDesde = fechaDesde.ToString("yyyy-MM-dd"),
                fechaHasta = fechaHasta.ToString("yyyy-MM-dd")
            };

            var jsonBody = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var res = await _http.PostAsync("/api/Reportes/AuditoriaOcupacion", content);
            if (!res.IsSuccessStatusCode)
                return null;

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReportesOperativoDto>(json);
        }
    }
}
