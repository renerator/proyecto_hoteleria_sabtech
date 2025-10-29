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
using static System.Net.WebRequestMethods;


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

        // GET /api/Inventarios/InventariosDisponibles?vigencia={vigencia}

       
        public async Task<List<InventarioDto>> InventarioDisponiblesAsync(int vigencia, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync($"/api/Inventarios/InventariosDisponibles?vigencia={vigencia}"))
                {
                    if ((int)resp.StatusCode == 204) return new List<InventarioDto>();
                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<InventarioDto>>(json) ?? new List<InventarioDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[InventariosDisponiblesAsync] {ex}");
                return new List<InventarioDto>();
            }
        }

        // GET /api/Inventarios/dashboardInventarios
        public async Task<InventarioDashboardDto> DashboardInventarioAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync("/api/Inventarios/dashboardInventarios"))
                {
                    if ((int)resp.StatusCode == 204) return new InventarioDashboardDto();
                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InventarioDashboardDto>(json) ?? new InventarioDashboardDto();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DashboardInventariosAsync] {ex}");
                return new InventarioDashboardDto();
            }
        }

        // POST /api/Inventarios/SolicitaInventario
        public async Task<bool> CrearInventarioAsync(InventarioDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                 var resp = await _http.PostAsync("/api/Inventarios/SolicitaInventario", content);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[CrearInventarioAsync] {ex}");
                return false;
            }
        }

        // POST /api/Inventarios/ConfirmarInventario
        public async Task<bool> ConfirmarInventarioAsync(InventarioDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await _http.PostAsync("/api/Inventarios/ConfirmarInventario", content);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ConfirmarInventarioAsync] {ex}");
                return false;
            }
        }

        // PUT /api/Inventarios/ModificaInventario
        public async Task<bool> ModificarInventarioAsync(InventarioDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
               var content = new StringContent(json, Encoding.UTF8, "application/json");
                 var resp = await _http.PutAsync("/api/Inventarios/ModificaInventario", content);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ModificarInventarioAsync] {ex}");
                return false;
            }
        }

        // DELETE /api/Inventarios/EliminaInventario?idInventario={id}
        public async Task<bool> EliminarInventarioAsync(int idInventario, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                 var resp = await _http.DeleteAsync($"/api/Inventarios/EliminaInventario?idInventario={idInventario}");
                if (resp.IsSuccessStatusCode) return true;

                var error = await resp.Content.ReadAsStringAsync();
                Trace.TraceWarning($"[EliminarInventarioAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {error}");
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[EliminarInventarioAsync] {ex}");
                return false;
            }
        }

        // GET /api/Inventarios/BuscarInventarios?criterio={texto}
        public async Task<List<InventarioDto>> BuscarInventarioAsync(string criterio, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var url = $"/api/Inventarios/BuscarInventarios?criterio={Uri.EscapeDataString(criterio ?? string.Empty)}";
                 var resp = await _http.GetAsync(url);
                if ((int)resp.StatusCode == 204) return new List<InventarioDto>();

                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<InventarioDto>>(json) ?? new List<InventarioDto>();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[BuscarInventariosAsync] {ex}");
                return new List<InventarioDto>();
            }
        }

       



       

        // Si más adelante usas bitácora:
        // public async Task<bool> CrearBitacoraInventarioAsync(BitacoraInventarioDto dto, string bearer = null) { ... }
    }
}
