// Front_Hoteleria/Services/Servicio/ServicioService.cs
using Front_Hoteleria.Dto.Servicio;
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

namespace Front_Hoteleria.Services.Servicio
{
    public class ServicioService : IServicioService
    {
        private static readonly HttpClient _http;

        static ServicioService()
        {
            var baseUrl = ConfigurationManager.AppSettings["Api.BaseUrl"]
                          ?? ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Falta Api.BaseUrl en Web.config (o ApiBaseUrl).");

            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
                throw new InvalidOperationException("Api.BaseUrl no es una URL válida: " + baseUrl);

            _http = new HttpClient { BaseAddress = baseUri, Timeout = TimeSpan.FromSeconds(30) };
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static void SetBearer(string bearer)
        {
            _http.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrWhiteSpace(bearer))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        // GET /api/Servicio/ListarServicios?estado={0|1}
        public async Task<List<ServicioDto>> ListarServiciosAsync(int? estado = null, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var url = "/api/Servicio/ListarServicios";
                if (estado.HasValue) url += "?vigencia=" + estado.Value;

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<ServicioDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ListarServiciosAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<ServicioDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return new List<ServicioDto>();

                    return JsonConvert.DeserializeObject<List<ServicioDto>>(json) ?? new List<ServicioDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ListarServiciosAsync] {ex}");
                return new List<ServicioDto>();
            }
        }

        // POST /api/Servicio/CrearServicio
        public async Task<bool> CrearServicioAsync(ServicioDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync("/api/Servicio/CrearServicio", content))
                {
                    // Acepta 200/201/204
                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[CrearServicioAsync] {ex}");
                return false;
            }
        }

        // PUT /api/Servicio/ModificarServicio
        public async Task<bool> ModificarServicioAsync(ServicioDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PutAsync("/api/Servicio/ModificarServicio", content))
                {
                    return resp.IsSuccessStatusCode; // incluye 204
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ModificarServicioAsync] {ex}");
                return false;
            }
        }

        // DELETE /api/Servicio/EliminarServicio?idServicio={id}
        public async Task<bool> EliminarServicioAsync(int idServicio, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var url = "/api/Servicio/EliminarServicio?idServicio=" + idServicio;

                using (var resp = await _http.DeleteAsync(url))
                {
                    if (resp.IsSuccessStatusCode) return true;

                    var error = await resp.Content.ReadAsStringAsync();
                    Trace.TraceWarning($"[EliminarServicioAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[EliminarServicioAsync] {ex}");
                return false;
            }
        }
    }
}
