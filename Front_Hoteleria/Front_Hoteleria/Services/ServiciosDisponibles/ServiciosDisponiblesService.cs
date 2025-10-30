// Front_Hoteleria/Services/Servicio/ServicioApiClient.cs
using Front_Hoteleria.Dto.Servicio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.ServiciosDisponibles
{
    public class ServiciosDisponiblesService : IServicioDisponiblesService
    {
        private static readonly HttpClient _http;

        static ServiciosDisponiblesService()
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

        public async Task<List<ServicioDto>> ListarServiciosAsync(int? estado = null, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var url = "/api/Servicio/ListarServicios";
                if (estado.HasValue) url += "?estado=" + estado.Value;

                using (var resp = await _http.GetAsync(url))
                {
                    if ((int)resp.StatusCode == 204) // NoContent
                        return new List<ServicioDto>();

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ServicioDto>>(json) ?? new List<ServicioDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ListarServiciosAsync] {ex}");
                return new List<ServicioDto>();
            }
        }

        public async Task<bool> CrearServicioAsync(ServicioDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync("/api/Servicio/CrearServicio", content))
                {
                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[CrearServicioAsync] {ex}");
                return false;
            }
        }

        public async Task<bool> ModificarServicioAsync(ServicioDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PutAsync("/api/Servicio/ModificarServicio", content))
                {
                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ModificarServicioAsync] {ex}");
                return false;
            }
        }

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

