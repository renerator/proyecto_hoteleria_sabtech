using Front_Hoteleria.Dto.Checkin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(bearer))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        public async Task<List<ReservaCheckinDto>> ListarReservasAsync(DateTime? fecha, string estado, string bearer = null)
        {
            SetBearer(bearer);

            // ajusta el endpoint según tu API real
            var url = "/api/CheckinCheckout/Listar";
            var query = new List<string>();
            if (fecha.HasValue) query.Add("fecha=" + fecha.Value.ToString("yyyy-MM-dd"));
            if (!string.IsNullOrWhiteSpace(estado)) query.Add("estado=" + estado);
            if (query.Count > 0) url += "?" + string.Join("&", query);

            var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode)
                return new List<ReservaCheckinDto>();

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ReservaCheckinDto>>(json)
                   ?? new List<ReservaCheckinDto>();
        }

        public async Task<CheckinKpiDto> KpiAsync(DateTime? fecha, string bearer = null)
        {
            SetBearer(bearer);
            var url = "/api/CheckinCheckout/Kpi";
            if (fecha.HasValue)
                url += "?fecha=" + fecha.Value.ToString("yyyy-MM-dd");

            var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode)
                return new CheckinKpiDto();

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CheckinKpiDto>(json) ?? new CheckinKpiDto();
        }

        public async Task<bool> HacerCheckinAsync(CheckinAccionDto dto, string bearer = null)
        {
            SetBearer(bearer);
            var json = JsonConvert.SerializeObject(dto);
            var res = await _http.PostAsync("/api/CheckinCheckout/Checkin",
                new StringContent(json, Encoding.UTF8, "application/json"));
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> HacerCheckoutAsync(CheckinAccionDto dto, string bearer = null)
        {
            SetBearer(bearer);
            var json = JsonConvert.SerializeObject(dto);
            var res = await _http.PostAsync("/api/CheckinCheckout/Checkout",
                new StringContent(json, Encoding.UTF8, "application/json"));
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> RegistrarNoShowAsync(CheckinAccionDto dto, string bearer = null)
        {
            SetBearer(bearer);
            var json = JsonConvert.SerializeObject(dto);
            var res = await _http.PostAsync("/api/CheckinCheckout/NoShow",
                new StringContent(json, Encoding.UTF8, "application/json"));
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> ExtenderReservaAsync(CheckinExtensionDto dto, string bearer = null)
        {
            SetBearer(bearer);
            var json = JsonConvert.SerializeObject(dto);
            var res = await _http.PostAsync("/api/CheckinCheckout/Extender",
                new StringContent(json, Encoding.UTF8, "application/json"));
            return res.IsSuccessStatusCode;
        }
    }
}
