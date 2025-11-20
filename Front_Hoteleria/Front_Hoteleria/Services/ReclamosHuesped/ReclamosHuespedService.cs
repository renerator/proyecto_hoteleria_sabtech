using Front_Hoteleria.Dto.Huesped;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.ReclamosHuesped
{
    public class ReclamosHuespedService : IReclamosHuespedService
    {
        private static readonly HttpClient _http;

        static ReclamosHuespedService()
        {
            var baseUrl = ConfigurationManager.AppSettings["Api.BaseUrl"]
                          ?? ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Falta Api.BaseUrl en Web.config (o ApiBaseUrl).");

            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
                throw new InvalidOperationException("Api.BaseUrl no es una URL válida: " + baseUrl);

            _http = new HttpClient
            {
                BaseAddress = baseUri,
                Timeout = TimeSpan.FromSeconds(30)
            };

            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static void SetBearer(string bearer)
        {
            _http.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(bearer))
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearer);
        }

        // =========================
        // LISTAR RECLAMOS / SUGERENCIAS
        // =========================
        // GET /api/ReclamosHuesped/Listar
        public async Task<List<ReclamoSolicitudDto>> ListarReclamosHuespedAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var url = "/api/Huesped/ListarReclamos"; // ajusta si tu API usa otro path

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<ReclamoSolicitudDto>();

                    resp.EnsureSuccessStatusCode();

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ReclamoSolicitudDto>>(json)
                           ?? new List<ReclamoSolicitudDto>();
                }
            }
            catch (HttpRequestException httpEx)
            {
                Trace.TraceError($"[ListarReclamosHuespedAsync] HTTP: {httpEx}");
                return new List<ReclamoSolicitudDto>();
            }
            catch (JsonException jsonEx)
            {
                Trace.TraceError($"[ListarReclamosHuespedAsync] JSON: {jsonEx}");
                return new List<ReclamoSolicitudDto>();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ListarReclamosHuespedAsync] {ex}");
                return new List<ReclamoSolicitudDto>();
            }
        }

        // GET /api/ReclamosHuesped/Reclamo/{id}
        public async Task<ReclamoSolicitudDto> ObtenerReclamoHuespedPorIdAsync(
            int idReclamoHuesped, string bearer)
        {
            try
            {
                SetBearer(bearer);
                var url = $"/api/Huesped/ObtenerReclamo/{idReclamoHuesped}";

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent ||
                        resp.StatusCode == HttpStatusCode.NotFound)
                        return null;

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ReclamoSolicitudDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ObtenerReclamoHuespedPorIdAsync] " + ex);
                return null;
            }
        }

        // =========================
        // CREAR RECLAMO / SUGERENCIA
        // =========================
        // POST /api/ReclamosHuesped/Crear
        public async Task<bool> CrearReclamoHuespedAsync(ReclamoSolicitudDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var url = "/api/Huesped/CrearReclamo"; // ajusta si tu API usa otro path

                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync(url, content))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return true;

                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[CrearReclamoHuespedAsync] {ex}");
                return false;
            }
        }
    }
}
