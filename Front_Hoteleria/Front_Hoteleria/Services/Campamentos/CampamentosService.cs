using Front_Hoteleria.Dto.Campamentos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Campamentos
{
    public class CampamentosService : ICampamentosService
    {
        private static readonly HttpClient _http;

        static CampamentosService()
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
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        // ===== KPI =====
        public async Task<CampamentoKpiDto> ResumenAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync("/api/Campamentos/resumen"))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new CampamentoKpiDto();

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CampamentoKpiDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CampamentosService.ResumenAsync] " + ex);

                // demo
                return new CampamentoKpiDto();
                //{
                //    //TotalCampamentos = 4,
                //    //TotalAreas = 12,
                //    //TotalHabitaciones = 156,
                //    //TasaUtilizacion = 78
                //};
            }
        }

        // ===== LISTAR =====
        public async Task<List<CampamentoDto>> ListarAsync(string criterio = null, string estado = null, string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var qs = new List<string>();
                if (!string.IsNullOrWhiteSpace(criterio))
                    qs.Add("criterio=" + Uri.EscapeDataString(criterio));
                if (!string.IsNullOrWhiteSpace(estado))
                    qs.Add("estado=" + Uri.EscapeDataString(estado));

                var url = "/api/Campamentos";
                if (qs.Count > 0)
                    url += "?" + string.Join("&", qs);

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<CampamentoDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[CampamentosService.ListarAsync] {(int)resp.StatusCode} -> {err}");
                        return new List<CampamentoDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<CampamentoDto>>(json)
                           ?? new List<CampamentoDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CampamentosService.ListarAsync] " + ex);

                // demo igual a tu HTML
                return new List<CampamentoDto>
                {
                    new CampamentoDto {
                        IdCampamento = 1,
                        Nombre = "Campamento Norte",
                        Codigo = "CAMP-N-001",
                        Ubicacion = "Sector Norte, Mina Escondida",
                        Capacidad = 200,
                        OcupacionActual = 156,
                        Estado = "active",
                        Encargado = "Juan Pérez",
                        Areas = new List<CampamentoAreaDto>
                        {
                            new CampamentoAreaDto{ Nombre="Comedor Principal", Tipo="comedor", Capacidad=100, Estado="active"},
                            new CampamentoAreaDto{ Nombre="Lavandería Central", Tipo="lavanderia", Capacidad=50, Estado="active"},
                            new CampamentoAreaDto{ Nombre="Sala de Recreación", Tipo="recreacion", Capacidad=30, Estado="maintenance"},
                        }
                    },
                    new CampamentoDto {
                        IdCampamento = 2,
                        Nombre = "Campamento Sur",
                        Codigo = "CAMP-S-002",
                        Ubicacion = "Sector Sur, Mina Los Pelambres",
                        Capacidad = 150,
                        OcupacionActual = 120,
                        Estado = "active",
                        Encargado = "María González",
                        Areas = new List<CampamentoAreaDto>
                        {
                            new CampamentoAreaDto{ Nombre="Comedor Sur", Tipo="comedor", Capacidad=80, Estado="active"},
                            new CampamentoAreaDto{ Nombre="Gimnasio", Tipo="recreacion", Capacidad=20, Estado="active"},
                        }
                    }
                };
            }
        }

        // ===== OBTENER POR ID =====
        public async Task<CampamentoDto> ObtenerPorIdAsync(int id, string bearer = null)
        {
            if (id==0)
                return null;

            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync($"/api/Campamentos/{id}"))
                {
                    if (resp.StatusCode == HttpStatusCode.NotFound ||
                        resp.StatusCode == HttpStatusCode.NoContent)
                        return null;

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CampamentoDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CampamentosService.ObtenerPorIdAsync] " + ex);
                return null;
            }
        }

        // ===== CREAR =====
        public async Task<bool> CrearAsync(CampamentoDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await _http.PostAsync("/api/Campamentos/CrearCampamento", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[CampamentosService.CrearAsync] {(int)resp.StatusCode} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CampamentosService.CrearAsync] " + ex);
                return false;
            }
        }

        // ===== ACTUALIZAR =====
        public async Task<bool> ActualizarAsync(CampamentoDto dto, string bearer = null)
        {
            if (dto == null || dto.IdCampamento==0)
                return false;

            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await _http.PutAsync($"/api/Campamentos/EditarCampamento/{dto.IdCampamento}", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[CampamentosService.ActualizarAsync] {(int)resp.StatusCode} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CampamentosService.ActualizarAsync] " + ex);
                return false;
            }
        }

        // ===== ELIMINAR =====
        public async Task<bool> EliminarAsync(int id, string bearer = null)
        {
            if (id==0)
                return false;

            try
            {
                SetBearer(bearer);
                using (var resp = await _http.DeleteAsync($"/api/Campamentos/EliminarCampamento/{id}"))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[CampamentosService.EliminarAsync] {(int)resp.StatusCode} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CampamentosService.EliminarAsync] " + ex);
                return false;
            }
        }

        // Front_Hoteleria/Services/Campamentos/CampamentosService.cs
        public async Task<List<CampamentoDto>> ListarComboAsync(
            bool? soloActivos = null,
            string filtro = null,
            string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var qs = new List<string>();
                if (soloActivos.HasValue)
                    qs.Add("soloActivos=" + (soloActivos.Value ? "true" : "false"));
                if (!string.IsNullOrWhiteSpace(filtro))
                    qs.Add("filtro=" + Uri.EscapeDataString(filtro));

                var url = "/api/Campamentos/combo";
                if (qs.Count > 0)
                    url += "?" + string.Join("&", qs);

                using (var resp = await _http.GetAsync(url))
                {
                    Trace.TraceInformation($"GET {resp?.RequestMessage?.RequestUri} -> {(int)resp.StatusCode}");

                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<CampamentoDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[CampamentosService.ListarComboAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<CampamentoDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<CampamentoDto>>(json) ?? new List<CampamentoDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CampamentosService.ListarComboAsync] " + ex);
                return new List<CampamentoDto>();
            }
        }

    }
}
