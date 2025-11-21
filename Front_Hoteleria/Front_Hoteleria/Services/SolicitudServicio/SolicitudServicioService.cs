using Front_Hoteleria.Dto.SolicitudServicio;
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

namespace Front_Hoteleria.Services.SolicitudServicio
{
    public class SolicitudServicioService : ISolicitudServicioService
    {
        private static readonly HttpClient _http;

        static SolicitudServicioService()
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

        // ===================== LISTA VIGENTES =====================
        public async Task<List<SolicitudServicioDto>> ListarSolicitudesVigentesAsync(
     DateTime? fechaInicio,
     DateTime? fechaFin,
     int idEstado,
     string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                // Armamos el querystring: vigencia + fechas si vienen
                var qs = new List<string> { "idEstado=" + idEstado };

                if (fechaInicio.HasValue)
                    qs.Add("fechaInicio=" + fechaInicio.Value.ToString("yyyy-MM-dd"));

                if (fechaFin.HasValue)
                    qs.Add("fechaFin=" + fechaFin.Value.ToString("yyyy-MM-dd"));

                var url = "/api/SolicitudServicio/ListaSolicitudesVigentes";
                if (qs.Count > 0)
                    url += "?" + string.Join("&", qs);

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<SolicitudServicioDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ListarSolicitudesVigentesAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<SolicitudServicioDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return new List<SolicitudServicioDto>();

                    return JsonConvert.DeserializeObject<List<SolicitudServicioDto>>(json)
                           ?? new List<SolicitudServicioDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ListarSolicitudesVigentesAsync] {ex}");
                return new List<SolicitudServicioDto>();
            }
        }


        // ===================== BÚSQUEDA POR FILTROS =====================
        public async Task<List<SolicitudServicioDto>> BuscarSolicitudesAsync(
            int? idSolicitud,
            int? idHabitacion,
            int? idServicio,
            DateTime? desde,
            DateTime? hasta,
            string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var qs = new List<string>();
                if (idSolicitud.HasValue) qs.Add("idSolicitud=" + idSolicitud.Value);
                if (idHabitacion.HasValue) qs.Add("idHabitacion=" + idHabitacion.Value);
                if (idServicio.HasValue) qs.Add("idServicio=" + idServicio.Value);
                if (desde.HasValue) qs.Add("desde=" + desde.Value.ToString("yyyy-MM-dd"));
                if (hasta.HasValue) qs.Add("hasta=" + hasta.Value.ToString("yyyy-MM-dd"));

                var queryString = qs.Count > 0 ? "?" + string.Join("&", qs) : string.Empty;
                var url = "/api/SolicitudServicio/BuscarSolicitudes" + queryString;

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<SolicitudServicioDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[BuscarSolicitudesAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<SolicitudServicioDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return new List<SolicitudServicioDto>();

                    return JsonConvert.DeserializeObject<List<SolicitudServicioDto>>(json)
                           ?? new List<SolicitudServicioDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[BuscarSolicitudesAsync] {ex}");
                return new List<SolicitudServicioDto>();
            }
        }

        // ===================== OBTENER POR ID =====================
        public async Task<SolicitudServicioDto> ObtenerSolicitudAsync(int idSolicitud, string bearer = null)
        {
            try
            {
                if (idSolicitud <= 0) return null;

                SetBearer(bearer);
                var url = "/api/SolicitudServicio/Obtener?idSolicitud=" + idSolicitud;

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return null;

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ObtenerSolicitudAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return null;
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return null;

                    return JsonConvert.DeserializeObject<SolicitudServicioDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ObtenerSolicitudAsync] {ex}");
                return null;
            }
        }
        // =========================================================
        // KPI DASHBOARD SOLICITUDES
        // GET /api/SolicitudServicio/ObtenerKPI
        // =========================================================
        public async Task<SolicitudKPIDto> ObtenerKpiAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                using (var resp = await _http.GetAsync("/api/SolicitudServicio/ResumenKPI"))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new SolicitudKPIDto();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ObtenerKpiAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new SolicitudKPIDto();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return new SolicitudKPIDto();

                    return JsonConvert.DeserializeObject<SolicitudKPIDto>(json)
                           ?? new SolicitudKPIDto();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ObtenerKpiAsync] {ex}");
                return new SolicitudKPIDto();
            }
        }
        // ===================== CREAR =====================
        public async Task<bool> CrearSolicitudAsync(SolicitudServicioDto dto, string bearer = null)
        {
            try
            {
                if (dto == null) return false;

                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync("/api/SolicitudServicio/CrearSolicitud", content))
                {
                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[CrearSolicitudAsync] {ex}");
                return false;
            }
        }

        // ===================== MODIFICAR =====================
        public async Task<bool> ModificarSolicitudAsync(SolicitudServicioDto dto, string bearer = null)
        {
            try
            {
                if (dto == null || dto.IdSolicitud <= 0) return false;

                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PutAsync("/api/SolicitudServicio/ModificarSolicitud", content))
                {
                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ModificarSolicitudAsync] {ex}");
                return false;
            }
        }
        // ===================== ASIGNAR PERSONAL =====================
        public async Task<bool> AsignarPersonalAsync(
            int idSolicitud,
            int? idPersonal,
            bool asignacionAutomatica,
            string bearer = null)
        {
            try
            {
                if (idSolicitud <= 0) return false;

                SetBearer(bearer);

                // Payload que espera tu API (ajusta nombres si es necesario)
                var payload = new
                {
                    IdSolicitud = idSolicitud,
                    IdPersonal = idPersonal,
                    AsignacionAutomatica = asignacionAutomatica
                };

                var json = JsonConvert.SerializeObject(payload);

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync("/api/SolicitudServicio/AsignarPersonal", content))
                {
                    if (resp.IsSuccessStatusCode)
                        return true;

                    var err = await resp.Content.ReadAsStringAsync();
                    Trace.TraceWarning($"[AsignarPersonalAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[AsignarPersonalAsync] {ex}");
                return false;
            }
        }

        // ===================== ELIMINAR =====================
        public async Task<bool> EliminarSolicitudAsync(int idSolicitud, string bearer = null)
        {
            try
            {
                if (idSolicitud <= 0) return false;

                SetBearer(bearer);
                var url = "/api/SolicitudServicio/EliminarSolicitud?idSolicitud=" + idSolicitud;

                using (var resp = await _http.DeleteAsync(url))
                {
                    if (resp.IsSuccessStatusCode) return true;

                    var err = await resp.Content.ReadAsStringAsync();
                    Trace.TraceWarning($"[EliminarSolicitudAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[EliminarSolicitudAsync] {ex}");
                return false;
            }
        }
    }
}
