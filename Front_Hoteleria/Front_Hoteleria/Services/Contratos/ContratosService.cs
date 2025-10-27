using Front_Hoteleria.Model.Reserva;
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


namespace Front_Hoteleria.Services.Contratos
{
    public class ContratosService : IContratosService
    {
        private static readonly HttpClient _http;

        static ContratosService()
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

        // GET /api/Reservas/ReservasDisponibles?vigencia={vigencia}
        public async Task<List<ReservaModel>> ReservasDisponiblesAsync(int vigencia, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync($"/api/Reservas/ReservasDisponibles?vigencia={vigencia}"))
                {
                    if ((int)resp.StatusCode == 204) return new List<ReservaModel>();
                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ReservaModel>>(json) ?? new List<ReservaModel>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ReservasDisponiblesAsync] {ex}");
                return new List<ReservaModel>();
            }
        }

        // GET /api/Reservas/dashboardReservas
        public async Task<ReservaDashboardModel> DashboardReservasAsync(string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                using (var resp = await _http.GetAsync("/api/Reservas/dashboardReservas"))
                {
                    if ((int)resp.StatusCode == 204) return new ReservaDashboardModel();
                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ReservaDashboardModel>(json) ?? new ReservaDashboardModel();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DashboardReservasAsync] {ex}");
                return new ReservaDashboardModel();
            }
        }

        // POST /api/Reservas/SolicitaReserva
        public async Task<bool> CrearReservaAsync(ReservaModel dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                 var resp = await _http.PostAsync("/api/Reservas/SolicitaReserva", content);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[CrearReservaAsync] {ex}");
                return false;
            }
        }

        // POST /api/Reservas/ConfirmarReserva
        public async Task<bool> ConfirmarReservaAsync(ReservaModel dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await _http.PostAsync("/api/Reservas/ConfirmarReserva", content);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ConfirmarReservaAsync] {ex}");
                return false;
            }
        }

        // PUT /api/Reservas/ModificaReserva
        public async Task<bool> ModificarReservaAsync(ReservaModel dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var json = JsonConvert.SerializeObject(dto);
               var content = new StringContent(json, Encoding.UTF8, "application/json");
                 var resp = await _http.PutAsync("/api/Reservas/ModificaReserva", content);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ModificarReservaAsync] {ex}");
                return false;
            }
        }

        // DELETE /api/Reservas/EliminaReserva?idReserva={id}
        public async Task<bool> EliminarReservaAsync(int idReserva, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                 var resp = await _http.DeleteAsync($"/api/Reservas/EliminaReserva?idReserva={idReserva}");
                if (resp.IsSuccessStatusCode) return true;

                var error = await resp.Content.ReadAsStringAsync();
                Trace.TraceWarning($"[EliminarReservaAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {error}");
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[EliminarReservaAsync] {ex}");
                return false;
            }
        }

        // GET /api/Reservas/BuscarReservas?criterio={texto}
        public async Task<List<ReservaModel>> BuscarReservasAsync(string criterio, string bearer = null)
        {
            try
            {
                SetBearer(bearer);
                var url = $"/api/Reservas/BuscarReservas?criterio={Uri.EscapeDataString(criterio ?? string.Empty)}";
                 var resp = await _http.GetAsync(url);
                if ((int)resp.StatusCode == 204) return new List<ReservaModel>();

                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ReservaModel>>(json) ?? new List<ReservaModel>();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[BuscarReservasAsync] {ex}");
                return new List<ReservaModel>();
            }
        }

       



public async Task<List<ReservaTrabajadorModel>> ReservasDisponiblesTrabajadorAsync(ReservaTrabajadorModel reservaTrabajador, string bearer = null)
    {
        try
        {
            SetBearer(bearer);

            reservaTrabajador = new ReservaTrabajadorModel();

            var basePath = "/api/Reservas/ReservasTrabajadorDisponibles";
            var sb = new StringBuilder(basePath);
            var first = true;

            // helper inline para agregar pares key=value
            Action<string, string> add = (k, v) =>
            {
                if (string.IsNullOrEmpty(v)) return;
                sb.Append(first ? "?" : "&");
                sb.Append(k).Append("=").Append(Uri.EscapeDataString(v));
                first = false;
            };

            if (reservaTrabajador.FechaDesde.HasValue)
                add("FechaDesde", reservaTrabajador.FechaDesde.Value.ToString("o")); // ISO-8601

            if (reservaTrabajador.FechaHasta.HasValue)
                add("FechaHasta", reservaTrabajador.FechaHasta.Value.ToString("o")); // ISO-8601
                                                                                     // Si quieres cierre inclusivo del día:
                                                                                     // add("FechaHasta", reservaTrabajador.FechaHasta.Value.Date.AddDays(1).AddTicks(-1).ToString("o"));

            if (reservaTrabajador.IdEstadoReserva > 0)
                add("idEstadoReserva", reservaTrabajador.IdEstadoReserva.ToString());

            if (reservaTrabajador.IdTipoReserva > 0)
                add("idtiporeserva", reservaTrabajador.IdTipoReserva.ToString());

            var url = sb.ToString();

            using (var resp = await _http.GetAsync(url))
            {
                if (resp.StatusCode == HttpStatusCode.NoContent)
                    return new List<ReservaTrabajadorModel>();

                resp.EnsureSuccessStatusCode();

                var json = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ReservaTrabajadorModel>>(json)
                       ?? new List<ReservaTrabajadorModel>();
            }
        }
        catch (HttpRequestException httpEx)
        {
            Trace.TraceError($"[ReservasDisponiblesTrabajadorAsync] HTTP: {httpEx}");
            return new List<ReservaTrabajadorModel>();
        }
        catch (JsonException jsonEx)
        {
            Trace.TraceError($"[ReservasDisponiblesTrabajadorAsync] JSON: {jsonEx}");
            return new List<ReservaTrabajadorModel>();
        }
        catch (Exception ex)
        {
            Trace.TraceError($"[ReservasDisponiblesTrabajadorAsync] {ex}");
            return new List<ReservaTrabajadorModel>();
        }
    }
        public async Task<bool> CrearReservaTrabajadorAsync(ReservaTrabajadorModel dto, string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                // Ajusta la ruta si tu API usa otra: p.ej. "/api/Reservas/CrearReserva"
                var url = "/api/Reservas/CreaReservaTrabajador";

                var payload = JsonConvert.SerializeObject(dto);
                using (var content = new StringContent(payload, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync(url, content))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent) return true;
                    if (!resp.IsSuccessStatusCode) return false;
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"[CrearReservaAsync] {ex}");
                return false;
            }
        }


        // Si más adelante usas bitácora:
        // public async Task<bool> CrearBitacoraReservaAsync(BitacoraReservaModel dto, string bearer = null) { ... }
    }
}
