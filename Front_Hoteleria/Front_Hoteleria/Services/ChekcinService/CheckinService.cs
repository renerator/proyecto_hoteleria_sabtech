using Front_Hoteleria.Dto.Checkin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Checkin
{
    public class CheckinService : ICheckinService
    {
        private static readonly HttpClient _http;

        static CheckinService()
        {
            _http = new HttpClient();
            var baseUrl = ConfigurationManager.AppSettings["Api.BaseUrl"]
                       ?? ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (!string.IsNullOrWhiteSpace(baseUrl))
                _http.BaseAddress = new Uri(baseUrl);
        }

        private void SetBearer(string bearer)
        {
            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(bearer))
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearer);
        }

        public async Task<List<ReservaCheckinDto>> ListarReservasAsync(
            DateTime? fecha,
            int idEstado,
            string bearer = null)
        {
            SetBearer(bearer);

            var url = "/api/Check/ReservasCheck";
            var query = new List<string>();

            if (fecha.HasValue)
                query.Add("fechadesde=" + fecha.Value.ToString("yyyy-MM-dd"));

            if (idEstado > 0)
                query.Add("idEstadoReserva=" + idEstado);

            if (query.Count > 0)
                url += "?" + string.Join("&", query);

            using (var res = await _http.GetAsync(url))
            {
                if (!res.IsSuccessStatusCode)
                    return new List<ReservaCheckinDto>();

                var json = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ReservaCheckinDto>>(json)
                       ?? new List<ReservaCheckinDto>();
            }
        }

        public async Task<CheckinKpiDto> KpiAsync(DateTime? fecha, string bearer = null)
        {
            SetBearer(bearer);
            var url = "/api/Check/ResumenCheckKPI";

            if (fecha.HasValue)
                url += "?fecha=" + fecha.Value.ToString("yyyy-MM-dd");

            var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode)
                return new CheckinKpiDto();

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CheckinKpiDto>(json)
                   ?? new CheckinKpiDto();
        }
    }
}
