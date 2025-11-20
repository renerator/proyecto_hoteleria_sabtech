using Front_Hoteleria.Dto.Huesped;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.ReservasHuesped
{
    public class ReservaHuespedService : IReservaHuespedService
    {
        private static readonly HttpClient _http;

        static ReservaHuespedService()
        {
            // Lee la URL base de la API desde Web.config
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
        // LISTAR RESERVAS HUESPED
        // POST /api/Huesped/BuscarReserva
        // =========================
        public async Task<List<ReservaHuespedDto>> ListarReservasHuespedAsync(
    ReservaHuespedDto filtro,
    string bearer)
        {
            try
            {
                SetBearer(bearer);

                // Base de la URL
                var url = "/api/Huesped/BuscarReserva";

                // Construimos querystring a mano
                var queryParts = new List<string>();

                if (!string.IsNullOrWhiteSpace(filtro?.FiltroCodigo))
                    queryParts.Add("FiltroCodigo=" + WebUtility.UrlEncode(filtro.FiltroCodigo));

                if (filtro?.FiltroIdEstado != null)
                    queryParts.Add("FiltroIdEstado=" + filtro.FiltroIdEstado.Value);

                if (filtro?.FiltroDesde != null)
                    queryParts.Add("FiltroDesde=" + filtro.FiltroDesde.Value.ToString("yyyy-MM-dd"));

                if (filtro?.FiltroHasta != null)
                    queryParts.Add("FiltroHasta=" + filtro.FiltroHasta.Value.ToString("yyyy-MM-dd"));

                if (queryParts.Any())
                    url += "?" + string.Join("&", queryParts);

                // AHORA: GET, SIN BODY
                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<ReservaHuespedDto>();

                    resp.EnsureSuccessStatusCode();

                    var body = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ReservaHuespedDto>>(body)
                           ?? new List<ReservaHuespedDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ListarReservasHuespedAsync] {ex}");
                return new List<ReservaHuespedDto>();
            }
        }


        // =========================
        // OBTENER POR ID
        // GET /api/Huesped/ObtenerReserva/{id}
        // =========================
        public async Task<ReservaHuespedDto> ObtenerReservaHuespedPorIdAsync(
            int idReserva,
            string bearer)
        {
            if (idReserva <= 0) return null;

            try
            {
                SetBearer(bearer);
                var url = $"/api/Huesped/ObtenerReserva/{idReserva}";

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent ||
                        resp.StatusCode == HttpStatusCode.NotFound)
                        return null;

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ReservaHuespedDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ObtenerReservaHuespedPorIdAsync] {ex}");
                return null;
            }
        }

        public async Task<bool> RegistrarEncuestaAsync(EncuestaSatisfaccionDto dto, string bearer)
        {
            if (dto == null) return false;

            try
            {
                SetBearer(bearer);
                var url = "/api/Huesped/RegistrarEncuesta";

                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync(url, content))
                {
                    Trace.TraceInformation($"[RegistrarEncuestaAsync] POST {url} => {(int)resp.StatusCode}");

                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return true;

                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[RegistrarEncuestaAsync] " + ex);
                return false;
            }
        }

        // =========================
        // CREAR
        // POST /api/Huesped/CrearReserva
        // =========================
        public async Task<bool> CrearReservaHuespedAsync(
            ReservaHuespedDto dto,
            string bearer)
        {
            if (dto == null) return false;

            try
            {
                SetBearer(bearer);
                var url = "/api/Huesped/CrearReserva";

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
                Trace.TraceError($"[CrearReservaHuespedAsync] {ex}");
                return false;
            }
        }

        // =========================
        // ACTUALIZAR
        // PUT /api/Huesped/ActualizarReserva/{id}
        // =========================
        public async Task<bool> ActualizarReservaHuespedAsync(
            ReservaHuespedDto dto,
            string bearer)
        {
            if (dto == null || dto.IdReserva <= 0) return false;

            try
            {
                SetBearer(bearer);
                var url = $"/api/Huesped/ActualizarReserva/{dto.IdReserva}";

                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PutAsync(url, content))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return true;

                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ActualizarReservaHuespedAsync] {ex}");
                return false;
            }
        }

        // =========================
        // ELIMINAR
        // DELETE /api/Huesped/EliminarReserva/{id}
        // =========================
        public async Task<bool> EliminarReservaHuespedAsync(
    int idReserva,
    string bearer)
        {
            if (idReserva <= 0) return false;

            try
            {
                SetBearer(bearer);
                var url = $"/api/Huesped/EliminarReserva/{idReserva}";

                using (var resp = await _http.DeleteAsync(url))
                {
                    Trace.TraceInformation($"[EliminarReservaHuespedAsync] DELETE {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}");

                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return true;

                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[EliminarReservaHuespedAsync] {ex}");
                return false;
            }
        }

    }
}
