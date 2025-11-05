using Front_Hoteleria.Dto.Contrato;
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

namespace Front_Hoteleria.Services.Contratos
{
    public class ContratosService : IContratosService
    {
        private static readonly HttpClient _http;

        static ContratosService()
        {
            // igual que tu servicio de reservas
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

        // ========== 1) RESUMEN ==========
        // GET /api/Contratos/resumen
        public async Task<ContratoKPIDto> ResumenAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                using (var resp = await _http.GetAsync("/api/Contratos/resumen"))
                {
                    if ((int)resp.StatusCode == 204)
                        return new ContratoKPIDto();

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ContratoKPIDto>(json)
                           ?? new ContratoKPIDto();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ContratosService.ResumenAsync] {ex}");
                return new ContratoKPIDto();
            }
        }

        // ========== 2) LISTAR (para la tabla) ==========
        // GET /api/Contratos?criterio=...
        public async Task<List<ContratoDto>> ListarAsync(string criterio = null, string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var url = "/api/Contratos";
                if (!string.IsNullOrWhiteSpace(criterio))
                    url += "?criterio=" + Uri.EscapeDataString(criterio);

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<ContratoDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ContratosService.ListarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<ContratoDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ContratoDto>>(json)
                           ?? new List<ContratoDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ContratosService.ListarAsync] {ex}");
                return new List<ContratoDto>();
            }
        }

        // ========== 3) OBTENER POR ID (para el modal) ==========
        // GET /api/Contratos/{id}
        public async Task<ContratoDto> ObtenerPorIdAsync(int id, string bearer = null)
        {
            try
            {
                if (id <= 0) return null;

                SetBearer(bearer);

                using (var resp = await _http.GetAsync($"/api/Contratos/{id}"))
                {
                    if (resp.StatusCode == HttpStatusCode.NotFound ||
                        resp.StatusCode == HttpStatusCode.NoContent)
                        return null;

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ContratosService.ObtenerPorIdAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return null;
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ContratoDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ContratosService.ObtenerPorIdAsync] {ex}");
                return null;
            }
        }

        // ========== 4) CREAR ==========
        // POST /api/Contratos
        public async Task<bool> CrearAsync(ContratoDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await _http.PostAsync("/api/Contratos", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ContratosService.CrearAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ContratosService.CrearAsync] {ex}");
                return false;
            }
        }

        // ========== 5) ACTUALIZAR ==========
        // PUT /api/Contratos/{id}
        public async Task<bool> ActualizarAsync(ContratoDto dto, string bearer = null)
        {
            try
            {
                if (dto == null || dto.IdContrato <= 0)
                    return false;

                SetBearer(bearer);

                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await _http.PutAsync($"/api/Contratos/{dto.IdContrato}", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ContratosService.ActualizarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ContratosService.ActualizarAsync] {ex}");
                return false;
            }
        }

        // ========== 6) ELIMINAR ==========
        // DELETE /api/Contratos/{id}
        public async Task<bool> EliminarAsync(int id, string bearer = null)
        {
            try
            {
                if (id <= 0) return false;

                SetBearer(bearer);

                using (var resp = await _http.DeleteAsync($"/api/Contratos/{id}"))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ContratosService.EliminarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ContratosService.EliminarAsync] {ex}");
                return false;
            }
        }
    }
}
