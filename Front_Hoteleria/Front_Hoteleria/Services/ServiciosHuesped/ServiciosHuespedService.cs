using Front_Hoteleria.Dto.Huesped;
using Front_Hoteleria.Dto.Reserva;
using Front_Hoteleria.Dto.Servicio;
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

namespace Front_Hoteleria.Services.ServiciosHuesped
{
    // OJO: ahora implementa IServiciosHuespedService  (plural)
    public class ServicioHuespedService : IServiciosHuespedService
    {
        private readonly HttpClient _http;
        private const string BasePath = "/api/Huesped";

        public ServicioHuespedService()
        {
            _http = new HttpClient();

            var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl))
                _http.BaseAddress = new Uri(baseUrl);

            _http.Timeout = TimeSpan.FromSeconds(30);
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

        public async Task<List<ServicioHuespedDto>> ListarServiciosHuespedAsync(
            ServicioHuespedDto filtro,
            string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var json = JsonConvert.SerializeObject(filtro ?? new ServicioHuespedDto());

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync($"{BasePath}/BuscarServicios", content))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent ||
                        resp.StatusCode == HttpStatusCode.NotFound)
                        return new List<ServicioHuespedDto>();

                    resp.EnsureSuccessStatusCode();
                    var str = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ServicioHuespedDto>>(str);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ServicioHuespedService.ListarServiciosHuespedAsync] " + ex);
                return new List<ServicioHuespedDto>();
            }
        }

        public async Task<ServicioHuespedDto> ObtenerServicioHuespedPorIdAsync(
            int idSolicitud,
            string bearer = null)
        {
            if (idSolicitud <= 0) return null;

            try
            {
                SetBearer(bearer);

                using (var resp = await _http.GetAsync($"{BasePath}/Servicio/{idSolicitud}"))
                {
                    if (resp.StatusCode == HttpStatusCode.NotFound ||
                        resp.StatusCode == HttpStatusCode.NoContent)
                        return null;

                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServicioHuespedDto>(json);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ServicioHuespedService.ObtenerServicioHuespedPorIdAsync] " + ex);
                return null;
            }
        }

        public async Task<bool> CrearServicioHuespedAsync(
            ServicioHuespedDto dto,
            string bearer = null)
        {
            if (dto == null) return false;

            try
            {
                SetBearer(bearer);

                var json = JsonConvert.SerializeObject(dto);

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync($"{BasePath}/CrearServicio", content))
                {
                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ServicioHuespedService.CrearServicioHuespedAsync] " + ex);
                return false;
            }
        }

        public async Task<bool> ActualizarServicioHuespedAsync(
            ServicioHuespedDto dto,
            string bearer = null)
        {
            if (dto == null || dto.IdSolicitudServicio <= 0) return false;

            try
            {
                SetBearer(bearer);

                var json = JsonConvert.SerializeObject(dto);

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PutAsync($"{BasePath}/ActualizarServicio", content))
                {
                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ServicioHuespedService.ActualizarServicioHuespedAsync] " + ex);
                return false;
            }
        }

        public async Task<bool> EliminarServicioHuespedAsync(
            int idSolicitud,
            string bearer = null)
        {
            if (idSolicitud <= 0) return false;

            try
            {
                SetBearer(bearer);

                using (var resp = await _http.DeleteAsync($"{BasePath}/EliminarServicio/{idSolicitud}"))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return true;

                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ServicioHuespedService.EliminarServicioHuespedAsync] " + ex);
                return false;
            }
        }
    }
}
