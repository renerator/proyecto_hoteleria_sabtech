using Front_Hoteleria.Dto.Roles;
using Microsoft.Ajax.Utilities;
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

namespace Front_Hoteleria.Services.Roles
{
    public class RolesService : IRolesService
    {
        private static readonly HttpClient _http;

        static RolesService()
        {
            var baseUrl = ConfigurationManager.AppSettings["Api.BaseUrl"]
                          ?? ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Falta Api.BaseUrl en Web.config (o ApiBaseUrl).");

            _http = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
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

        // ===== 1) Resumen =====
        // GET /api/Roles/resumen
        public async Task<RolesKpiDto> ResumenAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync("/api/Roles/resumen"))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new RolesKpiDto();

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<RolesKpiDto>(json)
                           ?? new RolesKpiDto();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[RolesService.ResumenAsync] " + ex);
                // demo
                return new RolesKpiDto
                {
                    TotalRoles = 5,
                    Administradores = 3,
                    Supervisores = 8,
                    Trabajadores = 145
                };
            }
        }

        // ===== 2) Listar =====
        // GET /api/Roles?criterio=...
        public async Task<List<RolDto>> ListarAsync(string criterio = null, string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var url = "/api/Roles";
                if (!string.IsNullOrWhiteSpace(criterio))
                    url += "?criterio=" + Uri.EscapeDataString(criterio);

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<RolDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[RolesService.ListarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<RolDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<RolDto>>(json)
                           ?? new List<RolDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[RolesService.ListarAsync] " + ex);

                // demo igual a la maqueta
                return new List<RolDto>
                {
                    new RolDto {
                        Id = 1,
                        Nombre = "Administrador",
                        Codigo = "ADMIN",
                        Descripcion = "Acceso completo a todas las funcionalidades del sistema",
                        UsuariosAsignados = 3,
                        Permisos = BuildDefaultPermisos()
                    },
                    new RolDto {
                        Id =2,
                        Nombre = "Supervisor",
                        Codigo = "SUPERVISOR",
                        Descripcion = "Acceso de supervisión a operaciones diarias",
                        UsuariosAsignados = 8,
                        Permisos = BuildDefaultPermisos()
                    },
                    new RolDto {
                        Id = 3,
                        Nombre = "Trabajador",
                        Codigo = "WORKER",
                        Descripcion = "Acceso básico para operaciones de campo",
                        UsuariosAsignados = 145,
                        Permisos = new List<RolPermisoDto>
                        {
                            new RolPermisoDto{ Codigo="rooms", Nombre="Gestión de Habitaciones", Habilitado=true },
                            new RolPermisoDto{ Codigo="services", Nombre="Gestión de Servicios", Habilitado=true }
                        }
                    }
                };
            }
        }

        // ===== 3) Obtener por id =====
        // GET /api/Roles/{id}
        public async Task<RolDto> ObtenerPorIdAsync(int id, string bearer = null)
        {
            if (id==0)
                return null;

            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync($"/api/Roles/{id}"))
                {
                    if (resp.StatusCode == HttpStatusCode.NotFound ||
                        resp.StatusCode == HttpStatusCode.NoContent)
                        return null;

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<RolDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[RolesService.ObtenerPorIdAsync] " + ex);
                return null;
            }
        }

        // ===== 4) Crear =====
        // POST /api/Roles
        public async Task<bool> CrearAsync(RolDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await _http.PostAsync("/api/Roles", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[RolesService.CrearAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[RolesService.CrearAsync] " + ex);
                return false;
            }
        }

        // ===== 5) Actualizar =====
        // PUT /api/Roles/{id}
        public async Task<bool> ActualizarAsync(RolDto dto, string bearer = null)
        {
            if (dto == null || dto.Id==0)
                return false;

            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await _http.PutAsync($"/api/Roles/{dto.Id}", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[RolesService.ActualizarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[RolesService.ActualizarAsync] " + ex);
                return false;
            }
        }

        // ===== 6) Eliminar =====
        // DELETE /api/Roles/{id}
        public async Task<bool> EliminarAsync(int id, string bearer = null)
        {
            if (id==0)
                return false;

            try
            {
                SetBearer(bearer);
                using (var resp = await _http.DeleteAsync($"/api/Roles/{id}"))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[RolesService.EliminarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[RolesService.EliminarAsync] " + ex);
                return false;
            }
        }

        // mismo helper que en el controller
        private static List<RolPermisoDto> BuildDefaultPermisos()
        {
            return new List<RolPermisoDto>
            {
                new RolPermisoDto{ Codigo = "rooms",       Nombre = "Gestión de Habitaciones",      Habilitado = true },
                new RolPermisoDto{ Codigo = "reservations",Nombre = "Gestión de Reservas",          Habilitado = true },
                new RolPermisoDto{ Codigo = "services",    Nombre = "Gestión de Servicios",         Habilitado = true },
                new RolPermisoDto{ Codigo = "camps",       Nombre = "Gestión de Campamentos",       Habilitado = true },
                new RolPermisoDto{ Codigo = "contracts",   Nombre = "Gestión de Contratos",         Habilitado = true },
                new RolPermisoDto{ Codigo = "staff",       Nombre = "Gestión de Dotaciones",        Habilitado = true },
                new RolPermisoDto{ Codigo = "roles",       Nombre = "Gestión de Roles",             Habilitado = true },
                new RolPermisoDto{ Codigo = "reports",     Nombre = "Reportes y Estadísticas",      Habilitado = true },
            };
        }
    }
}
