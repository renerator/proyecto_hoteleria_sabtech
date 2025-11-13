using Front_Hoteleria.Dto.Dotaciones;
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

namespace Front_Hoteleria.Services.Dotaciones
{
    public class DotacionesService : IDotacionesService
    {
        // HttpClient compartido como en CampamentosService
        private static readonly HttpClient _http;

        static DotacionesService()
        {
            var baseUrl = ConfigurationManager.AppSettings["Api.BaseUrl"]
                          ?? ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Falta Api.BaseUrl en Web.config.");

            _http = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
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
        // 1. KPI / Resumen
        // GET /api/Dotaciones/resumen
        // =========================
        public async Task<DotacionKPIDto> ResumenAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                using (var resp = await _http.GetAsync("/api/Dotaciones/resumen"))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new DotacionKPIDto();

                    resp.EnsureSuccessStatusCode();

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<DotacionKPIDto>(json)
                           ?? new DotacionKPIDto();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[DotacionesService.ResumenAsync] " + ex);
                // devolvemos algo vacío para que el front no reviente
                return new DotacionKPIDto();
            }
        }

        // =========================
        // 2. Listar
        // GET /api/Dotaciones?empresaId=1&filtro=juan
        // =========================
        public async Task<List<DotacionDto>> ListarAsync(int? empresaId = null, string filtro = null, string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var qs = new List<string>();
                if (empresaId.HasValue)
                    qs.Add("empresaId=" + empresaId.Value);
                if (!string.IsNullOrWhiteSpace(filtro))
                    qs.Add("filtro=" + Uri.EscapeDataString(filtro));

                var url = "/api/Dotaciones";
                if (qs.Count > 0)
                    url += "?" + string.Join("&", qs);

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<DotacionDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[DotacionesService.ListarAsync] {(int)resp.StatusCode} -> {err}");
                        return new List<DotacionDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<DotacionDto>>(json)
                           ?? new List<DotacionDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[DotacionesService.ListarAsync] " + ex);
                return new List<DotacionDto>();
            }
        }

        // =========================
        // 3. Obtener por Id
        // GET /api/Dotaciones/{id}
        // =========================
        public async Task<DotacionDto> ObtenerPorIdAsync(int id, string bearer = null)
        {
            if (id <= 0)
                return null;

            try
            {
                SetBearer(bearer);

                using (var resp = await _http.GetAsync($"/api/Dotaciones/{id}"))
                {
                    if (resp.StatusCode == HttpStatusCode.NotFound ||
                        resp.StatusCode == HttpStatusCode.NoContent)
                        return null;

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[DotacionesService.ObtenerPorIdAsync] {(int)resp.StatusCode} -> {err}");
                        return null;
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<DotacionDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[DotacionesService.ObtenerPorIdAsync] " + ex);
                return null;
            }
        }

        // =========================
        // 4. Crear
        // POST /api/Dotaciones/CrearDotacion
        // (si tu backend usa solo POST /api/Dotaciones, cambia la ruta abajo)
        // =========================
        public async Task<bool> CrearAsync(DotacionDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // ajusta la URL si en tu API es distinta
                using (var resp = await _http.PostAsync("/api/Dotaciones/CrearDotacion", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[DotacionesService.CrearAsync] {(int)resp.StatusCode} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[DotacionesService.CrearAsync] " + ex);
                return false;
            }
        }

        // =========================
        // 5. Actualizar
        // PUT /api/Dotaciones/EditarDotacion/{id}
        // =========================
        public async Task<bool> ModificarAsync(DotacionDto dto, string bearer = null)
        {
            if (dto == null || dto.IdDotacion <= 0)
                return false;

            try
            {
                SetBearer(bearer);

                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await _http.PutAsync($"/api/Dotaciones/EditarDotacion/{dto.IdDotacion}", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[DotacionesService.ModificarAsync] {(int)resp.StatusCode} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[DotacionesService.ModificarAsync] " + ex);
                return false;
            }
        }

        // =========================
        // 6. Eliminar
        // DELETE /api/Dotaciones/EliminarDotacion/{id}
        // =========================
        public async Task<bool> EliminarAsync(int id, string bearer = null)
        {
            if (id <= 0)
                return false;

            try
            {
                SetBearer(bearer);

                using (var resp = await _http.DeleteAsync($"/api/Dotaciones/EliminarDotacion/{id}"))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[DotacionesService.EliminarAsync] {(int)resp.StatusCode} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[DotacionesService.EliminarAsync] " + ex);
                return false;
            }
        }
    }
}
