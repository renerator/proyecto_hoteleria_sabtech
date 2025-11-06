using Front_Hoteleria.Dto.ServiciosPersonal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.ServiciosPersonal
{
    public class ServiciosPersonalService : IServiciosPersonalService
    {
        private static readonly HttpClient _http;

        static ServiciosPersonalService()
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

        public async Task<ServiciosPersonalKpiDto> ObtenerKpiAsync(string bearer = null)
        {
            SetBearer(bearer);

            // cambia al endpoint real
            var res = await _http.GetAsync("/api/ServiciosPersonal/Kpi");
            if (!res.IsSuccessStatusCode)
                return new ServiciosPersonalKpiDto();

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ServiciosPersonalKpiDto>(json)
                   ?? new ServiciosPersonalKpiDto();
        }

        public async Task<List<ServiciosPersonalDto>> ListarSolicitudesAsync(
            string ordenarPor = null,
            string prioridad = null,
            string estado = null,
            string ubicacion = null,
            string bearer = null)
        {
            SetBearer(bearer);
            var url = "/api/ServiciosPersonal/Solicitudes";
            var q = new List<string>();
            if (!string.IsNullOrWhiteSpace(ordenarPor)) q.Add("ordenarPor=" + ordenarPor);
            if (!string.IsNullOrWhiteSpace(prioridad)) q.Add("prioridad=" + prioridad);
            if (!string.IsNullOrWhiteSpace(estado)) q.Add("estado=" + estado);
            if (!string.IsNullOrWhiteSpace(ubicacion)) q.Add("ubicacion=" + Uri.EscapeDataString(ubicacion));
            if (q.Count > 0) url += "?" + string.Join("&", q);

            var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode)
                return new List<ServiciosPersonalDto>();

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ServiciosPersonalDto>>(json)
                   ?? new List<ServiciosPersonalDto>();
        }

        public async Task<List<ServiciosPersonalDto>> ListarServiciosActivosAsync(string bearer = null)
        {
            SetBearer(bearer);
            var res = await _http.GetAsync("/api/ServiciosPersonal/Activos");
            if (!res.IsSuccessStatusCode)
                return new List<ServiciosPersonalDto>();

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ServiciosPersonalDto>>(json)
                   ?? new List<ServiciosPersonalDto>();
        }

        public async Task<List<ServiciosPersonalDto>> ListarProximasSolicitudesAsync(string bearer = null)
        {
            SetBearer(bearer);
            var res = await _http.GetAsync("/api/ServiciosPersonal/Proximos");
            if (!res.IsSuccessStatusCode)
                return new List<ServiciosPersonalDto>();

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ServiciosPersonalDto>>(json)
                   ?? new List<ServiciosPersonalDto>();
        }

        public async Task<bool> AsignarSolicitudAsync(string id, string bearer = null)
        {
            SetBearer(bearer);
            var body = JsonConvert.SerializeObject(new { id });
            var res = await _http.PostAsync("/api/ServiciosPersonal/Asignar",
                new StringContent(body, Encoding.UTF8, "application/json"));
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> IniciarSolicitudAsync(string id, string tiempoEstimado, string observaciones, string bearer = null)
        {
            SetBearer(bearer);
            var body = JsonConvert.SerializeObject(new
            {
                id,
                tiempoEstimado,
                observaciones
            });
            var res = await _http.PostAsync("/api/ServiciosPersonal/Iniciar",
                new StringContent(body, Encoding.UTF8, "application/json"));
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> CompletarServicioAsync(string id, string descripcion, string bearer = null)
        {
            SetBearer(bearer);
            var body = JsonConvert.SerializeObject(new
            {
                id,
                descripcion
            });
            var res = await _http.PostAsync("/api/ServiciosPersonal/Completar",
                new StringContent(body, Encoding.UTF8, "application/json"));
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> NotificarHuespedAsync(string id, string metodo, string destino, string mensaje, string bearer = null)
        {
            SetBearer(bearer);
            var body = JsonConvert.SerializeObject(new
            {
                id,
                metodo,
                destino,
                mensaje
            });
            var res = await _http.PostAsync("/api/ServiciosPersonal/Notificar",
                new StringContent(body, Encoding.UTF8, "application/json"));
            return res.IsSuccessStatusCode;
        }
    }
}
