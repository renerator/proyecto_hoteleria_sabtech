using Front_Hoteleria.Dto.Calendario;
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

namespace Front_Hoteleria.Services.Calendario
{
    public class CalendarioService : ICalendarioService
    {
        private static readonly HttpClient _http;

        static CalendarioService()
        {
            var baseUrl = ConfigurationManager.AppSettings["Api.BaseUrl"]
                          ?? ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Falta Api.BaseUrl en Web.config");

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
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearer);
        }
        public async Task<List<string>> ListarHabitacionesAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                // ruta ejemplo, ajústala a tu API real
                using (var resp = await _http.GetAsync("/api/calendario/habitaciones"))
                {
                    if (!resp.IsSuccessStatusCode)
                        throw new Exception("no ok");

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<string>>(json);
                }
            }
            catch
            {
                // datos de demo
                var list = new List<string>();
                for (int i = 1; i <= 20; i++)
                    list.Add(i.ToString("D4"));
                return list;
            }
        }

        public async Task<bool> BloquearHabitacionAsync(CalendarioBloqueoDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // ruta ejemplo
                using (var resp = await _http.PostAsync("/api/calendario/bloqueos", content))
                {
                    return resp.IsSuccessStatusCode;
                }
            }
            catch
            {
                // en maqueta devolvemos true
                return true;
            }
        }
        public async Task<bool> ProgramarMantenimientoAsync(CalendarioMantenimientoDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // ajusta la ruta a la de tu API real
                using (var resp = await _http.PostAsync("/api/calendario/mantenimientos", content))
                {
                    // si tu API aún no existe, devolvemos true igual
                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("[CalendarioService.ProgramarMantenimientoAsync] " + ex);
                return true; // para la maqueta
            }
        }

        public async Task<CalendarioKpiDto> ResumenAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync("/api/Calendario/resumen"))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return GetDummyKpi();

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CalendarioKpiDto>(json)
                           ?? GetDummyKpi();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CalendarioService.ResumenAsync] " + ex);
                return GetDummyKpi();
            }
        }

        public async Task<List<CalendarioEventoDto>> ListarAsync(
            string habitacion = null,
            string estado = null,
            string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var qs = new List<string>();
                if (!string.IsNullOrWhiteSpace(habitacion))
                    qs.Add("habitacion=" + Uri.EscapeDataString(habitacion));
                if (!string.IsNullOrWhiteSpace(estado))
                    qs.Add("estado=" + Uri.EscapeDataString(estado));

                var url = "/api/Calendario";
                if (qs.Count > 0)
                    url += "?" + string.Join("&", qs);

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return GetDummyEvents();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[CalendarioService.ListarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return GetDummyEvents();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<CalendarioEventoDto>>(json)
                           ?? GetDummyEvents();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CalendarioService.ListarAsync] " + ex);
                return GetDummyEvents();
            }
        }

        public async Task<CalendarioEventoDto> ObtenerPorIdAsync(string id, string bearer = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync($"/api/Calendario/{id}"))
                {
                    if (resp.StatusCode == HttpStatusCode.NotFound ||
                        resp.StatusCode == HttpStatusCode.NoContent)
                        return null;

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CalendarioEventoDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CalendarioService.ObtenerPorIdAsync] " + ex);
                return null;
            }
        }

        public async Task<bool> CrearAsync(CalendarioEventoDto dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                using (var resp = await _http.PostAsync("/api/Calendario", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[CalendarioService.CrearAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CalendarioService.CrearAsync] " + ex);
                return false;
            }
        }

        public async Task<bool> ActualizarAsync(CalendarioEventoDto dto, string bearer = null)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
                return false;

            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                using (var resp = await _http.PutAsync($"/api/Calendario/{dto.Id}", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[CalendarioService.ActualizarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CalendarioService.ActualizarAsync] " + ex);
                return false;
            }
        }

        public async Task<bool> EliminarAsync(string id, string bearer = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            try
            {
                SetBearer(bearer);
                using (var resp = await _http.DeleteAsync($"/api/Calendario/{id}"))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[CalendarioService.EliminarAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[CalendarioService.EliminarAsync] " + ex);
                return false;
            }
        }

        // ======== datos dummy ========
        private CalendarioKpiDto GetDummyKpi() => new CalendarioKpiDto
        {
            TotalHabitaciones = 20,
            OcupadasHoy = 89,
            EnMantenimiento = 12,
            EnSanitizacion = 8
        };

        private List<CalendarioEventoDto> GetDummyEvents()
        {
            var hoy = DateTime.Today;
            return new List<CalendarioEventoDto>
            {
                new CalendarioEventoDto {
                    Id = "CAL-001",
                    HabitacionId = "0001",
                    HabitacionNombre = "0001",
                    Titulo = "Ocupada",
                    FechaInicio = hoy.AddDays(1),
                    FechaFin = hoy.AddDays(3),
                    Tipo = "occupied",
                    Descripcion = "Reserva confirmada",
                    Color = "#d9534f"
                },
                new CalendarioEventoDto {
                    Id = "CAL-002",
                    HabitacionId = "0002",
                    HabitacionNombre = "0002",
                    Titulo = "Mantenimiento",
                    FechaInicio = hoy.AddDays(2),
                    FechaFin = hoy.AddDays(2),
                    Tipo = "maintenance",
                    Descripcion = "Mantenimiento preventivo",
                    Color = "#f0ad4e"
                },
                new CalendarioEventoDto {
                    Id = "CAL-003",
                    HabitacionId = "0003",
                    HabitacionNombre = "0003",
                    Titulo = "Sanitización",
                    FechaInicio = hoy.AddDays(2).AddHours(10),
                    FechaFin = hoy.AddDays(2).AddHours(14),
                    Tipo = "sanitization",
                    Descripcion = "Limpieza profunda",
                    Color = "#5bc0de"
                },
                new CalendarioEventoDto {
                    Id = "CAL-004",
                    HabitacionId = "0005",
                    HabitacionNombre = "0005",
                    Titulo = "Bloqueada",
                    FechaInicio = hoy.AddDays(3),
                    FechaFin = hoy.AddDays(5),
                    Tipo = "blocked",
                    Descripcion = "Reparación de plomería",
                    Color = "#6c757d"
                }
            };
        }
    }
}
