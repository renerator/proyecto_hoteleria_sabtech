using Front_Hoteleria.Dto.Huesped;
using Front_Hoteleria.Dto.Reserva;
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
using static System.Net.WebRequestMethods;

namespace Front_Hoteleria.Services.ServiciosHuesped
{
    // Implementa IServiciosHuespedService
    public class ServicioHuespedService : IServiciosHuespedService
    {
        private static readonly HttpClient _http;
        private const string BasePath = "/api/Huesped";

        // ========= CTOR ESTÁTICO, MISMO ESTILO QUE ReservaService =========
        static ServicioHuespedService()
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

        // =========================================================
        // 1) LISTAR SERVICIOS DEL HUÉSPED
        // POST /api/Huesped/BuscarServicios
        // =========================================================
       

    public async Task<List<ServicioHuespedDto>> ListarServiciosHuespedAsync(
    ServicioHuespedDto filtro,
    string bearer = null)
    {
        try
        {
            SetBearer(bearer);

            filtro = new ServicioHuespedDto();

            var qs = new List<string>();

            // Mapeamos los campos de filtro del DTO a parámetros de querystring
            if (filtro.FiltroIdEstado.HasValue)
                qs.Add("idEstado=" + filtro.FiltroIdEstado.Value);

            if (filtro.FiltroDesde.HasValue)
                qs.Add("desde=" + filtro.FiltroDesde.Value.ToString("yyyy-MM-dd"));

            if (filtro.FiltroHasta.HasValue)
                qs.Add("hasta=" + filtro.FiltroHasta.Value.ToString("yyyy-MM-dd"));

            if (!string.IsNullOrWhiteSpace(filtro.FiltroNombreServicio))
                qs.Add("nombreServicio=" + WebUtility.UrlEncode(filtro.FiltroNombreServicio));

            if (!string.IsNullOrWhiteSpace(filtro.FiltroTexto))
                qs.Add("texto=" + WebUtility.UrlEncode(filtro.FiltroTexto));

            var url = $"{BasePath}/BuscarServicios";
            if (qs.Count > 0)
                url += "?" + string.Join("&", qs);

            using (var resp = await _http.GetAsync(url))
            {
                if (resp.StatusCode == HttpStatusCode.NoContent ||
                    resp.StatusCode == HttpStatusCode.NotFound)
                    return new List<ServicioHuespedDto>();

                if (!resp.IsSuccessStatusCode)
                {
                    var err = await resp.Content.ReadAsStringAsync();
                    Trace.TraceWarning(
                        $"[ServicioHuespedService.ListarServiciosHuespedAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                    return new List<ServicioHuespedDto>();
                }

                var str = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ServicioHuespedDto>>(str)
                       ?? new List<ServicioHuespedDto>();
            }
        }
        catch (Exception ex)
        {
            Trace.TraceError("[ServicioHuespedService.ListarServiciosHuespedAsync] " + ex);
            return new List<ServicioHuespedDto>();
        }
    }

    // =========================================================
    // 2) OBTENER POR ID
    // GET /api/Huesped/Servicio/{id}
    // =========================================================
    public async Task<ServicioHuespedDto> ObtenerServicioHuespedPorIdAsync(
            int idSolicitud,
            string bearer = null)
        {
            if (idSolicitud <= 0) return null;

            try
            {
                SetBearer(bearer);

                using (var resp = await _http.GetAsync($"{BasePath}/Servicio/{idSolicitud}"))
                {
                    if (resp.StatusCode == HttpStatusCode.NotFound ||
                        resp.StatusCode == HttpStatusCode.NoContent)
                        return null;

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServicioHuespedDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ServicioHuespedService.ObtenerServicioHuespedPorIdAsync] " + ex);
                return null;
            }
        }

        // =========================================================
        // 3) CREAR SERVICIO
        // POST /api/Huesped/CrearServicio
        // =========================================================
        public async Task<bool> CrearServicioHuespedAsync(
            ServicioHuespedDto dto,
            string bearer = null)
        {
            if (dto == null) return false;

            try
            {
                SetBearer(bearer);

                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync($"{BasePath}/CrearServicio", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning(
                            $"[ServicioHuespedService.CrearServicioHuespedAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ServicioHuespedService.CrearServicioHuespedAsync] " + ex);
                return false;
            }
        }

        // =========================================================
        // 4) ACTUALIZAR SERVICIO
        // PUT /api/Huesped/ActualizarServicio
        // =========================================================
        public async Task<bool> ActualizarServicioHuespedAsync(
            ServicioHuespedDto dto,
            string bearer = null)
        {
            if (dto == null || dto.IdSolicitudServicio <= 0) return false;

            try
            {
                SetBearer(bearer);

                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PutAsync($"{BasePath}/ActualizarServicio", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning(
                            $"[ServicioHuespedService.ActualizarServicioHuespedAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ServicioHuespedService.ActualizarServicioHuespedAsync] " + ex);
                return false;
            }
        }

        // =========================================================
        // 5) ELIMINAR SERVICIO
        // DELETE /api/Huesped/EliminarServicio/{id}
        // =========================================================
        public async Task<bool> EliminarServicioHuespedAsync(
            int idSolicitud,
            string bearer = null)
        {
            if (idSolicitud <= 0) return false;

            try
            {
                SetBearer(bearer);

                using (var resp = await _http.DeleteAsync($"{BasePath}/EliminarServicio/{idSolicitud}"))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return true;

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning(
                            $"[ServicioHuespedService.EliminarServicioHuespedAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ServicioHuespedService.EliminarServicioHuespedAsync] " + ex);
                return false;
            }
        }
    }
}
