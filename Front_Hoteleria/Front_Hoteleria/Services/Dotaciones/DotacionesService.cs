using Front_Hoteleria.Dto.Dotaciones;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Dotaciones
{
    public class DotacionesService : IDotacionesService
    {
        private readonly HttpClient _http;

        public DotacionesService()
        {
            // como en tus otros services
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://tuservidor/api/") // <--- cámbialo
            };
        }

        private void SetBearer(string bearer)
        {
            _http.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrWhiteSpace(bearer))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearer);
            }
        }

        // ===== RESUMEN (panel) =====
        public async Task<DotacionKPIDto> ResumenAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                // GET /api/dotaciones/resumen
                using (var resp = await _http.GetAsync("dotaciones/resumen"))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceWarning($"[DotacionService.ResumenAsync] {(int)resp.StatusCode} {resp.ReasonPhrase}");
                        return new DotacionKPIDto();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return new DotacionKPIDto();

                    return JsonConvert.DeserializeObject<DotacionKPIDto>(json)
                           ?? new DotacionKPIDto();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DotacionService.ResumenAsync] {ex}");
                return new DotacionKPIDto();
            }
        }

        // ===== LISTAR (para la tabla) =====
        public async Task<List<DotacionDto>> ListarAsync(int? empresaId = null, string filtro = null, string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var url = "dotaciones/listar"; // GET
                var qs = new List<string>();
                if (empresaId.HasValue) qs.Add("empresaId=" + empresaId.Value);
                if (!string.IsNullOrWhiteSpace(filtro)) qs.Add("filtro=" + Uri.EscapeDataString(filtro));
                if (qs.Count > 0) url += "?" + string.Join("&", qs);

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<DotacionDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[DotacionService.ListarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<DotacionDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return new List<DotacionDto>();

                    return JsonConvert.DeserializeObject<List<DotacionDto>>(json)
                           ?? new List<DotacionDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DotacionService.ListarAsync] {ex}");
                return new List<DotacionDto>();
            }
        }

        // ===== OBTENER POR ID (para editar) =====
        public async Task<DotacionDto> ObtenerPorIdAsync(int id, string bearer = null)
        {
            try
            {
                if (id <= 0) return null;

                SetBearer(bearer);

                // GET /api/dotaciones/{id}
                var url = $"dotaciones/{id}";

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NotFound ||
                        resp.StatusCode == HttpStatusCode.NoContent)
                        return null;

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[DotacionService.ObtenerPorIdAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return null;
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(json))
                        return null;

                    return JsonConvert.DeserializeObject<DotacionDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DotacionService.ObtenerPorIdAsync] {ex}");
                return null;
            }
        }

        // ===== CREAR =====
        public async Task<bool> CrearAsync(DotacionDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var jsonBody = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // POST /api/dotaciones
                using (var resp = await _http.PostAsync("dotaciones", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[DotacionService.CrearAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DotacionService.CrearAsync] {ex}");
                return false;
            }
        }

        // ===== MODIFICAR =====
        public async Task<bool> ModificarAsync(DotacionDto dto, string bearer = null)
        {
            try
            {
                if (dto == null || dto.IdDotacion <= 0)
                    return false;

                SetBearer(bearer);

                var jsonBody = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // PUT /api/dotaciones/{id}
                using (var resp = await _http.PutAsync($"dotaciones/{dto.IdDotacion}", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[DotacionService.ModificarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DotacionService.ModificarAsync] {ex}");
                return false;
            }
        }

        // ===== ELIMINAR =====
        public async Task<bool> EliminarAsync(int id, string bearer = null)
        {
            try
            {
                if (id <= 0) return false;

                SetBearer(bearer);

                // DELETE /api/dotaciones/{id}
                using (var resp = await _http.DeleteAsync($"dotaciones/{id}"))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[DotacionService.EliminarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DotacionService.EliminarAsync] {ex}");
                return false;
            }
        }
    }
}
