using Font_Hoteleria.Dto.Trabajadores;
using Front_Hoteleria.Dto.Roles;
using Front_Hoteleria.Services.Trabajadores;
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

namespace Front_Hoteleria.Services.Trabajadores
{
    public class TrabajadoresService : ITrabajadoresService
    {
        private static readonly HttpClient _http;

        static TrabajadoresService()
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

        /// <summary>
        /// POST /api/Trabajador/CrearTrabajador
        /// El backend devuelve un string (p.ej. "True OK, Datos insertados" o mensaje de error).
        /// Consideramos éxito si Status=200 y el cuerpo contiene "True" (case-insensitive).
        /// </summary>
        public async Task<bool> CrearAsync(TrabajadoresDto dto, string bearer = null)
        {
            if (dto == null) return false;

            try
            {
                SetBearer(bearer);

                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await _http.PostAsync("/api/Trabajador/CrearTrabajador", content))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return false;

                    var body = await resp.Content.ReadAsStringAsync();

                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceWarning($"[TrabajadoresService.CrearAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {body}");
                        return false;
                    }

                    // Backend retorna texto; aceptamos "True" en el cuerpo como éxito
                    return body?.IndexOf("true", StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[TrabajadoresService.CrearAsync] " + ex);
                return false;
            }


        }

        public async Task<List<TrabajadoresDto>> ListarAsync(int? IdEmpresa = null, string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var url = "/api/Trabajador/ListarTrabajadores";
                if (IdEmpresa.HasValue) url += "?IdEmpresa=" + IdEmpresa.Value;

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<TrabajadoresDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[TrabajadoresService.ListarAsync] {(int)resp.StatusCode} -> {err}");
                        return new List<TrabajadoresDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<TrabajadoresDto>>(json)
                           ?? new List<TrabajadoresDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[TrabajadoresService.ListarAsync] " + ex);
                return new List<TrabajadoresDto>();
            }
        }
    }
}
