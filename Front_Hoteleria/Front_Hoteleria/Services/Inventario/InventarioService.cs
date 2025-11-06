using Front_Hoteleria.Dto.Inventario;
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

namespace Front_Hoteleria.Services.Inventario
{
    public class InventarioService : IInventarioService
    {
        private static readonly HttpClient _http;

        static InventarioService()
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
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static void SetBearer(string bearer)
        {
            _http.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrWhiteSpace(bearer))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        // ========== 1) RESUMEN ==========
        // GET /api/Inventario/resumen
        public async Task<InventarioKpiDto> ResumenAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync("/api/Inventario/resumen"))
                {
                    if ((int)resp.StatusCode == 204)
                        return new InventarioKpiDto();

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InventarioKpiDto>(json)
                           ?? new InventarioKpiDto();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InventarioService.ResumenAsync] " + ex);
                // datos dummy por si la API no responde
                return new InventarioKpiDto
                {
                    TotalItems = 156,
                    Disponibles = 142,
                    Faltantes = 3,
                    EnMantenimiento = 11
                };
            }
        }

        // ========== 2) LISTAR ==========
        // GET /api/Inventario?criterio=...&categoria=...&estado=...&habitacion=...
        public async Task<List<InventarioItemDto>> ListarAsync(
            string criterio = null,
            string categoria = null,
            string estado = null,
            string habitacion = null,
            string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var qs = new List<string>();
                if (!string.IsNullOrWhiteSpace(criterio))
                    qs.Add("criterio=" + Uri.EscapeDataString(criterio));
                if (!string.IsNullOrWhiteSpace(categoria))
                    qs.Add("categoria=" + Uri.EscapeDataString(categoria));
                if (!string.IsNullOrWhiteSpace(estado))
                    qs.Add("estado=" + Uri.EscapeDataString(estado));
                if (!string.IsNullOrWhiteSpace(habitacion))
                    qs.Add("habitacion=" + Uri.EscapeDataString(habitacion));

                var url = "/api/Inventario";
                if (qs.Count > 0)
                    url += "?" + string.Join("&", qs);

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<InventarioItemDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[InventarioService.ListarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<InventarioItemDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<InventarioItemDto>>(json)
                           ?? new List<InventarioItemDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InventarioService.ListarAsync] " + ex);
                return new List<InventarioItemDto>();
            }
        }

        // ========== 3) OBTENER POR ID ==========
        // GET /api/Inventario/{id}
        public async Task<InventarioItemDto> ObtenerPorIdAsync(string id, string bearer = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync($"/api/Inventario/{id}"))
                {
                    if (resp.StatusCode == HttpStatusCode.NotFound ||
                        resp.StatusCode == HttpStatusCode.NoContent)
                        return null;

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InventarioItemDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InventarioService.ObtenerPorIdAsync] " + ex);
                return null;
            }
        }

        // ========== 4) CREAR ==========
        // POST /api/Inventario
        public async Task<bool> CrearAsync(InventarioItemDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await _http.PostAsync("/api/Inventario", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[InventarioService.CrearAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InventarioService.CrearAsync] " + ex);
                return false;
            }
        }

        // ========== 5) ACTUALIZAR ==========
        // PUT /api/Inventario/{id}
        public async Task<bool> ActualizarAsync(InventarioItemDto dto, string bearer = null)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
                    return false;

                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await _http.PutAsync($"/api/Inventario/{dto.Id}", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[InventarioService.ActualizarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InventarioService.ActualizarAsync] " + ex);
                return false;
            }
        }

        // ========== 6) ELIMINAR ==========
        // DELETE /api/Inventario/{id}
        public async Task<bool> EliminarAsync(string id, string bearer = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return false;

                SetBearer(bearer);
                using (var resp = await _http.DeleteAsync($"/api/Inventario/{id}"))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[InventarioService.EliminarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[InventarioService.EliminarAsync] " + ex);
                return false;
            }
        }
        // =========================================================
        // 1) OBTENER UN ARTÍCULO POR ID
        //    GET /api/Inventario/{id}
        // =========================================================
        public async Task<InventarioItemDto> GetByIdAsync(string id, string bearer = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            try
            {
                SetBearer(bearer);

                // ruta asumida
                var url = $"/api/Inventario/{Uri.EscapeDataString(id)}";

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent ||
                        resp.StatusCode == HttpStatusCode.NotFound)
                        return null;

                    resp.EnsureSuccessStatusCode();

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InventarioItemDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[InventarioService.GetByIdAsync] {ex}");
                return null;
            }
        }

        // =========================================================
        // 2) LISTAR MOVIMIENTOS DEL ARTÍCULO
        //    GET /api/Inventario/{id}/movimientos
        // =========================================================
        public async Task<List<InventarioMovimientoPostDto>> GetMovimientosAsync(string id, string bearer = null)
        {
            var listaVacia = new List<InventarioMovimientoPostDto>();

            if (string.IsNullOrWhiteSpace(id))
                return listaVacia;

            try
            {
                SetBearer(bearer);

                // ruta asumida
                var url = $"/api/Inventario/{Uri.EscapeDataString(id)}/movimientos";

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return listaVacia;

                    resp.EnsureSuccessStatusCode();

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<InventarioMovimientoPostDto>>(json)
                           ?? listaVacia;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[InventarioService.GetMovimientosAsync] {ex}");
                return listaVacia;
            }
        }
    }
}
