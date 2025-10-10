using Front_Hoteleria.Dto.Habitacion;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Front_Hoteleria.Services.Api
{
    public interface IHotelApiClient
    {
        Task<List<HabitacionDto>> HabitacionesDisponiblesAsync(int vigencia, string bearer = null);
        Task<bool> CrearHabitacionAsync(HabitacionDto dto, string bearer = null);
        Task<bool> ConfirmarHabitacionAsync(HabitacionDto dto, string bearer = null);
        Task<bool> ModificarHabitacionAsync(HabitacionDto dto, string bearer = null);
        Task<bool> EliminarHabitacionAsync(int idHabitacion, string bearer = null);
    }

    public class HotelApiClient : IHotelApiClient
    {
        private static readonly HttpClient _http;

        static HotelApiClient()
        {
            var baseUrl = ConfigurationManager.AppSettings["Api.BaseUrl"]
              ?? ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Falta Api.BaseUrl en Web.config (o ApiBaseUrl).");

            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
                throw new InvalidOperationException("Api.BaseUrl no es una URL válida: " + baseUrl);

            _http = new HttpClient { BaseAddress = baseUri };
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static void SetBearer(string bearer)
        {
            _http.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrWhiteSpace(bearer))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        public async Task<List<HabitacionDto>> HabitacionesDisponiblesAsync(int vigencia, string bearer = null)
        {
            SetBearer(bearer);

            var url = "/api/Habitacion/HabitacionesDisponibles?vigencia=" + vigencia;
            using (var resp = await _http.GetAsync(url))
            {
                if (resp.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return new List<HabitacionDto>();

                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<HabitacionDto>>(json) ?? new List<HabitacionDto>();
            }
        }

        public async Task<bool> CrearHabitacionAsync(HabitacionDto dto, string bearer = null)
        {
            SetBearer(bearer);

            var json = JsonConvert.SerializeObject(dto);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            using (var resp = await _http.PostAsync("/api/Habitacion/SolicitaHabitacion", content))
            {
                return resp.IsSuccessStatusCode;
            }
        }

        public async Task<bool> ConfirmarHabitacionAsync(HabitacionDto dto, string bearer = null)
        {
            SetBearer(bearer);

            var json = JsonConvert.SerializeObject(dto);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            using (var resp = await _http.PostAsync("/api/Habitacion/ConfirmarHabitacion", content))
            {
                return resp.IsSuccessStatusCode;
            }
        }

        public async Task<bool> ModificarHabitacionAsync(HabitacionDto dto, string bearer = null)
        {
            SetBearer(bearer);

            var json = JsonConvert.SerializeObject(dto);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            using (var resp = await _http.PutAsync("/api/Habitacion/ModificaHabitacion", content))
            {
                return resp.IsSuccessStatusCode;
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
                    if (resp.IsSuccessStatusCode)
                        return true;

                   
                    var error = await resp.Content.ReadAsStringAsync();
                    // Log.Error($"DELETE {url} -> {(int)resp.StatusCode} {resp.ReasonPhrase}: {error}");

                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                // Log.Error(ex, "Error de red al eliminar habitación {Id}", idHabitacion);
                return false;
            }
            catch (Exception ex)
            {
                // Log.Error(ex, "Error inesperado al eliminar habitación {Id}", idHabitacion);
                return false;
            }
        }
    }
}