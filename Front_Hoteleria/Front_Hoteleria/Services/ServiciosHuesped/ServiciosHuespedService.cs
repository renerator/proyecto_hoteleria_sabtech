using Front_Hoteleria.Dto.Reserva;
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

namespace Front_Hoteleria.Services.ServiciosHuesped
{
    public class ServicioHuespedService : IServiciosHuespedService
    {
        private static readonly HttpClient _http;

        static ServicioHuespedService()
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

        public async Task<List<ServicioDto>> HabitacionesDisponiblesAsync(int vigencia, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var url = "/api/Habitacion/HabitacionesDisponibles?vigencia=" + vigencia;

                using (var resp = await _http.GetAsync(url))
                {
                    if ((int)resp.StatusCode == 204) // NoContent
                        return new List<ServicioDto>();

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ServicioDto>>(json) ?? new List<ServicioDto>();
                }
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[HabitacionesDisponiblesAsync] Error HTTP: {ex}");
                return new List<ServicioDto>();
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[HabitacionesDisponiblesAsync] Timeout: {ex}");
                return new List<ServicioDto>();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[HabitacionesDisponiblesAsync] Error inesperado: {ex}");
                return new List<ServicioDto>();
            }
        }

        public async Task<ServicioDashboardDto> DashboardHabitacionAsync(DateTime? desde, DateTime? hasta, string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var qs = System.Web.HttpUtility.ParseQueryString(string.Empty);
                if (desde.HasValue) qs["desde"] = desde.Value.ToString("yyyy-MM-dd");
                if (hasta.HasValue) qs["hasta"] = hasta.Value.ToString("yyyy-MM-dd");

                var url = "/api/Habitacion/dashboardHabitacion";
                var query = qs.ToString();
                if (!string.IsNullOrEmpty(query)) url += "?" + query;

                using (var resp = await _http.GetAsync(url))
                {
                    if ((int)resp.StatusCode == 204)
                        return new ServicioDashboardDto();

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServicioDashboardDto>(json) ?? new ServicioDashboardDto();
                }
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[DashboardHabitacionAsync] Error HTTP: {ex}");
                return new ServicioDashboardDto();
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[DashboardHabitacionAsync] Timeout: {ex}");
                return new ServicioDashboardDto();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DashboardHabitacionAsync] Error inesperado: {ex}");
                return new ServicioDashboardDto();
            }
        }

        public async Task<bool> CrearHabitacionAsync(ServicioDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync("/api/Habitacion/SolicitaHabitacion", content))
                {
                    return resp.IsSuccessStatusCode;
                }
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[CrearHabitacionAsync] Error HTTP: {ex}");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[CrearHabitacionAsync] Timeout: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[CrearHabitacionAsync] Error inesperado: {ex}");
                return false;
            }
        }

        public async Task<bool> ConfirmarHabitacionAsync(ServicioDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync("/api/Habitacion/ConfirmarHabitacion", content))
                {
                    return resp.IsSuccessStatusCode;
                }
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[ConfirmarHabitacionAsync] Error HTTP: {ex}");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[ConfirmarHabitacionAsync] Timeout: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ConfirmarHabitacionAsync] Error inesperado: {ex}");
                return false;
            }
        }

        public async Task<bool> ModificarHabitacionAsync(ServicioDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PutAsync("/api/Habitacion/ModificaHabitacion", content))
                {
                    return resp.IsSuccessStatusCode;
                }
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[ModificarHabitacionAsync] Error HTTP: {ex}");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[ModificarHabitacionAsync] Timeout: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ModificarHabitacionAsync] Error inesperado: {ex}");
                return false;
            }
        }

        public async Task<bool> EliminarHabitacionAsync(int idHabitacion, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var url = "/api/Habitacion/EliminaHabitacion?idHabitacion=" + idHabitacion;

                using (var resp = await _http.DeleteAsync(url))
                {
                    if (resp.IsSuccessStatusCode) return true;

                    var error = await resp.Content.ReadAsStringAsync();
                    Trace.TraceWarning($"[EliminarHabitacionAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {error}");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[EliminarHabitacionAsync] Error HTTP: {ex}");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[EliminarHabitacionAsync] Timeout: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[EliminarHabitacionAsync] Error inesperado: {ex}");
                return false;
            }
        }
    }
}
