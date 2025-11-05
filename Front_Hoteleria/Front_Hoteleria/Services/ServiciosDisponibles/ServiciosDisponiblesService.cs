// Front_Hoteleria/Services/Servicio/ServicioService.cs
using Front_Hoteleria.Dto.ServicioCategoria;
using Front_Hoteleria.Dto.ServicioEstado;
using Front_Hoteleria.Dto.ServicioPrioridad;
using Front_Hoteleria.Dto.ServiciosDisponibles;
using Front_Hoteleria.Services.Servicio;
using Front_Hoteleria.Services.ServiciosDisponibles;
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

namespace Front_Hoteleria.Services.ServiciosDisponibles
{
    public class ServiciosDisponiblesService : IServiciosDisponiblesService
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

        public async Task<List<  ServicioDisponibleDto>> VerificaServicioPorId(  ServicioDisponibleDto servicio, string token)
        {
            try
            {
                // validación básica
                if (servicio == null || servicio.IdServicio <= 0)
                    return new List<  ServicioDisponibleDto>();

                // setea el bearer igual que en los otros métodos
                SetBearer(token);

                // tu endpoint que devuelve 1   ServicioDisponibleDto
                // ej: /api/Servicio/MuestraServicio?id=5
                var url = "/api/Servicio/MuestraServicio?id=" + servicio.IdServicio;

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<  ServicioDisponibleDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[VerificaServicioPorId] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<  ServicioDisponibleDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return new List<  ServicioDisponibleDto>();

                    // el endpoint devuelve UN SOLO dto
                    var dto = JsonConvert.DeserializeObject<  ServicioDisponibleDto>(json);
                    if (dto == null)
                        return new List<  ServicioDisponibleDto>();

                    // tu interfaz pide List<  ServicioDisponibleDto>
                    return new List<  ServicioDisponibleDto> { dto };
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[VerificaServicioPorId] {ex}");
                return new List<  ServicioDisponibleDto>();
            }
        }



        // GET /api/Servicio/ListarServicios?estado={0|1}
        public async Task<List<  ServicioDisponibleDto>> ListarServiciosAsync(int? estado = null, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var url = "/api/Servicio/ListarServicios";
                if (estado.HasValue) url += "?vigencia=" + estado.Value;

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<  ServicioDisponibleDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ListarServiciosAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<  ServicioDisponibleDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return new List<  ServicioDisponibleDto>();

                    return JsonConvert.DeserializeObject<List<  ServicioDisponibleDto>>(json) ?? new List<  ServicioDisponibleDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ListarServiciosAsync] {ex}");
                return new List<  ServicioDisponibleDto>();
            }
        }

        // POST /api/Servicio/CrearServicio
        public async Task<bool> CrearServicioAsync(  ServicioDisponibleDto dto, string bearer = null)
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
        public async Task<bool> ModificarServicioAsync(  ServicioDisponibleDto dto, string bearer = null)
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

        // =========================================================
        // 2) Combo: Estado
        // GET /api/Servicio/ListarServicioEstado?vigencia=1
        // =========================================================
        public async Task<List<ServicioEstadoDto>> ListarServicioEstadoAsync(int vigencia = 1, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var url = $"/api/Servicio/ListarServiciosEstados?vigencia={vigencia}";

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<ServicioEstadoDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ListarServicioEstadoAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<ServicioEstadoDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return new List<ServicioEstadoDto>();

                    return JsonConvert.DeserializeObject<List<ServicioEstadoDto>>(json)
                           ?? new List<ServicioEstadoDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ListarServicioEstadoAsync] {ex}");
                return new List<ServicioEstadoDto>();
            }
        }

        // =========================================================
        // 3) Combo: Categoría
        // GET /api/Servicio/ListarServiciosCategoria?vigencia=1
        // =========================================================
        public async Task<List<ServicioCategoriaDto>> ListarServiciosCategoriaAsync(int vigencia = 1, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var url = $"/api/Servicio/ListarServiciosCategoria?vigencia={vigencia}";

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<ServicioCategoriaDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ListarServiciosCategoriaAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<ServicioCategoriaDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return new List<ServicioCategoriaDto>();

                    return JsonConvert.DeserializeObject<List<ServicioCategoriaDto>>(json)
                           ?? new List<ServicioCategoriaDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ListarServiciosCategoriaAsync] {ex}");
                return new List<ServicioCategoriaDto>();
            }
        }

        // =========================================================
        // 4) Combo: Prioridad
        // GET /api/Servicio/ListarServicioPrioridad?vigencia=1
        // =========================================================
        public async Task<List<ServicioPrioridadDto>> ListarServicioPrioridadAsync(int vigencia = 1, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var url = $"/api/Servicio/ListarServicioPrioridad?vigencia={vigencia}";

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<ServicioPrioridadDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ListarServicioPrioridadAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<ServicioPrioridadDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return new List<ServicioPrioridadDto>();

                    return JsonConvert.DeserializeObject<List<ServicioPrioridadDto>>(json)
                           ?? new List<ServicioPrioridadDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ListarServicioPrioridadAsync] {ex}");
                return new List<ServicioPrioridadDto>();
            }
        }

        public async Task<ServicioKpiDto> KpiServiciosAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer); // igual que en tus otros métodos
                using (var resp = await _http.GetAsync("/api/Servicio/KpiServicios"))
                {
                    if ((int)resp.StatusCode == (int)HttpStatusCode.NoContent)
                        return new ServicioKpiDto();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[KpiServiciosAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new ServicioKpiDto();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return new ServicioKpiDto();

                    return JsonConvert.DeserializeObject<ServicioKpiDto>(json) ?? new ServicioKpiDto();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[KpiServiciosAsync] {ex}");
                return new ServicioKpiDto();
            }
        }
    }
}
// Front_Hoteleria/Services/Servicio/ServicioApiClient.cs
