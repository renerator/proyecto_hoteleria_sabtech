using Front_Hoteleria.Dto.Reserva;

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

namespace Front_Hoteleria.Services.Reservas
{
    public class ReservaService : IReservaService
    {
        private static readonly HttpClient _http;

        // ===== ctor estático igual que InventarioService =====
        static ReservaService()
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
        // 1) RESUMEN
        // GET /api/Reservas/resumen
        // =========================================================
        public async Task<ReservaKPIDto> ResumenAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync("/api/Reservas/Resumen"))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new ReservaKPIDto();

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ReservaKPIDto>(json)
                           ?? new ReservaKPIDto();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasService.ResumenAsync] " + ex);

                // demo por si la API cae
                return new ReservaKPIDto
                {
                    //Pendientes = 5,
                    //Confirmadas = 18,
                    //Rechazadas = 2,
                    //Total = 25
                };
            }
        }

        // =========================================================
        // 2) LISTAR
        // GET /api/Reservas?estado=...&habitacion=...&fechaDesde=...&fechaHasta=...
        // =========================================================
        public async Task<List<ReservaDto>> ListarAsync(
            int? estado = 0,
            //string habitacion = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var qs = new List<string>();
                //if (!string.IsNullOrWhiteSpace(estado))
                    qs.Add("idEstadoReserva=" + estado);                
                if (fechaDesde.HasValue)
                    qs.Add("fechaDesde=" + fechaDesde.Value.ToString("yyyy-MM-dd"));
                if (fechaHasta.HasValue)
                    qs.Add("fechaHasta=" + fechaHasta.Value.ToString("yyyy-MM-dd"));

                var url = "/api/Reservas/ReservasDisponibles";
                if (qs.Count > 0)
                    url += "?" + string.Join("&", qs);

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<ReservaDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ReservasService.ListarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<ReservaDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ReservaDto>>(json)
                           ?? new List<ReservaDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasService.ListarAsync] " + ex);
                // retornamos demo para que la tabla no muera
                return DemoReservas();
            }
        }

        // =========================================================
        // 3) OBTENER POR ID
        // GET /api/Reservas/{id}
        // =========================================================
        public async Task<ReservaDto> ObtenerPorIdAsync(int idReserva, string bearer = null)
        {
            if (idReserva == 0)
                return null;

            try
            {
                if (!string.IsNullOrWhiteSpace(bearer))
                    SetBearer(bearer);

                // 👇 OJO: ahora con ?idReserva= en lugar de /{idReserva}
                var url = $"/api/Reservas/MuestraReserva?idReserva={idReserva}";

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NotFound ||
                        resp.StatusCode == HttpStatusCode.NoContent)
                        return null;

                    resp.EnsureSuccessStatusCode();

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ReservaDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasService.ObtenerPorIdAsync] " + ex);
                // fallback demo
                return DemoReservas().Find(r => r.IdReserva == idReserva);
            }
        }


        // =========================================================
        // 4) CREAR
        // POST /api/Reservas
        // =========================================================
        public async Task<bool> CrearAsync(ReservaDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await _http.PostAsync("/api/Reservas/CrearReserva", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ReservasService.CrearAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasService.CrearAsync] " + ex);
                return false;
            }
        }

        // =========================================================
        // 5) ACTUALIZAR
        // PUT /api/Reservas/{id}
        // =========================================================
        public async Task<bool> ActualizarAsync(ReservaDto dto, string bearer = null)
        {
            if (dto == null || dto.IdReserva==0)
                return false;

            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await _http.PutAsync($"/api/Reservas/{dto.IdReserva}", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[ReservasService.ActualizarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasService.ActualizarAsync] " + ex);
                return false;
            }
        }

        // =========================================================
        // 6) ELIMINAR
        // DELETE /api/Reservas/{id}
        // =========================================================
        public async Task<bool> EliminarAsync(ReservaDto dto, string bearer = null)
        {
            if (dto == null || dto.IdReserva == 0)
                return false;

            try
            {
                SetBearer(bearer);

                var json = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var request = new HttpRequestMessage(HttpMethod.Delete, "/api/Reservas/RechazaReserva"))
                {
                    request.Content = content;

                    using (var resp = await _http.SendAsync(request))
                    {
                        if (!resp.IsSuccessStatusCode)
                        {
                            var err = await resp.Content.ReadAsStringAsync();
                            Trace.TraceWarning($"[ReservasService.EliminarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                            return false;
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasService.EliminarAsync] " + ex);
                return false;
            }
        }

        // =========================================================
        // 7) COMBO ESTADOS
        // GET /api/Reservas/estados
        // =========================================================
        public async Task<List<ComboItemDto>> EstadosAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync("/api/Reservas/estados"))
                {
                    if (!resp.IsSuccessStatusCode)
                        return DemoEstados();

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ComboItemDto>>(json)
                           ?? DemoEstados();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasService.EstadosAsync] " + ex);
                return DemoEstados();
            }
        }

        // =========================================================
        // 8) COMBO HABITACIONES
        // GET /api/Reservas/habitaciones
        // =========================================================
        public async Task<List<ComboItemDto>> HabitacionesAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync("/api/Reservas/habitaciones"))
                {
                    if (!resp.IsSuccessStatusCode)
                        return DemoHabitaciones();

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ComboItemDto>>(json)
                           ?? DemoHabitaciones();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasService.HabitacionesAsync] " + ex);
                return DemoHabitaciones();
            }
        }

        // =========================================================
        // 9) COMBO TIPOS HABITACIÓN
        // GET /api/Reservas/tipos
        // =========================================================
        public async Task<List<ComboItemDto>> TiposHabitacionAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync("/api/Reservas/tipos"))
                {
                    if (!resp.IsSuccessStatusCode)
                        return DemoTipos();

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ComboItemDto>>(json)
                           ?? DemoTipos();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReservasService.TiposHabitacionAsync] " + ex);
                return DemoTipos();
            }
        }

        // =========================================================
        // D A T O S   D E M O
        // =========================================================
        private List<ReservaDto> DemoReservas()
        {
            return new List<ReservaDto>
            {
                new ReservaDto{
                    IdReserva = 1,
                    FechaDesde = DateTime.Today.AddDays(1),
                    FechaHasta = DateTime.Today.AddDays(3),
                    HuespedNombre = "Juan Pérez",
                    HuespedEmail = "juan.perez@email.com",
                    HuespedTelefono = "+56912345678",
                    TipoHabitacionNombre = "Suite",
                    CantidadPersonas = 2,
                    Estado = "pendiente",
                    Observaciones = "Cliente preferencial"
                },
                new ReservaDto{
                    IdReserva = 1,
                    FechaDesde = DateTime.Today.AddDays(2),
                    FechaHasta = DateTime.Today.AddDays(5),
                    HuespedNombre = "María González",
                    HuespedEmail = "maria.g@email.com",
                    HuespedTelefono = "+56987654321",
                    TipoHabitacionNombre = "Doble",
                    CantidadPersonas = 2,
                    Estado = "confirmada"
                },
                new ReservaDto{
                    IdReserva = 3,
                    FechaDesde = DateTime.Today.AddDays(4),
                    FechaHasta = DateTime.Today.AddDays(6),
                    HuespedNombre = "Carlos Rodríguez",
                    HuespedEmail = "carlos.r@email.com",
                    TipoHabitacionNombre = "Individual",
                    CantidadPersonas = 1,
                    Estado = "rechazada"
                }
            };
        }

        private List<ComboItemDto> DemoEstados()
        {
            return new List<ComboItemDto>
            {
                new ComboItemDto{ Id="1", Value="pendiente",  Text="Pendiente" },
                new ComboItemDto{ Id="2", Value="confirmada", Text="Confirmada" },
                new ComboItemDto{ Id="3", Value="rechazada",  Text="Rechazada" },
                new ComboItemDto{ Id="4", Value="realizada",  Text="Realizada" },
            };
        }

        private List<ComboItemDto> DemoHabitaciones()
        {
            return new List<ComboItemDto>
            {
                new ComboItemDto{ Id="101", Value="101", Text="Hab. 101 - Individual" },
                new ComboItemDto{ Id="201", Value="201", Text="Hab. 201 - Doble" },
                new ComboItemDto{ Id="301", Value="301", Text="Hab. 301 - Suite" },
            };
        }

        private List<ComboItemDto> DemoTipos()
        {
            return new List<ComboItemDto>
            {
                new ComboItemDto{ Id="1", Value="Suite",      Text="Suite" },
                new ComboItemDto{ Id="2", Value="Doble",      Text="Doble" },
                new ComboItemDto{ Id="3", Value="Individual", Text="Individual" },
                new ComboItemDto{ Id="4", Value="Familiar",   Text="Familiar" },
            };
        }
    }
}
