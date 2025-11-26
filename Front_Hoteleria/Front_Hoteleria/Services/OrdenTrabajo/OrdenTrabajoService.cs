using Front_Hoteleria.Dto.OrdenTrabajo;

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

namespace Front_Hoteleria.Services.OrdenTrabajo
{
    public class OrdenTrabajoService : IOrdenTrabajoService
    {
        private static readonly HttpClient _http;

        // ===== ctor estático igual que InventarioService =====
        static OrdenTrabajoService()
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
        //public async Task<ReservaKPIDto> ResumenAsync(string bearer = null)
        //{
        //    try
        //    {
        //        SetBearer(bearer);
        //        using (var resp = await _http.GetAsync("/api/Reservas/Resumen"))
        //        {
        //            if (resp.StatusCode == HttpStatusCode.NoContent)
        //                return new ReservaKPIDto();

        //            resp.EnsureSuccessStatusCode();
        //            var json = await resp.Content.ReadAsStringAsync();
        //            return JsonConvert.DeserializeObject<ReservaKPIDto>(json)
        //                   ?? new ReservaKPIDto();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceError("[ReservasService.ResumenAsync] " + ex);

        //        // demo por si la API cae
        //        return new ReservaKPIDto
        //        {
        //            //Pendientes = 5,
        //            //Confirmadas = 18,
        //            //Rechazadas = 2,
        //            //Total = 25
        //        };
        //    }
        //}

        // =========================================================
        // 2) LISTAR
        // GET /api/Reservas?estado=...&habitacion=...&fechaDesde=...&fechaHasta=...
        // =========================================================
        // =========================================================
        // 2) LISTAR ORDENES POR VIGENCIA
        // GET /api/OrdenTrabajo/ListaPorVigencia?vigencia=1
        // =========================================================
        public async Task<List<OrdenTrabajoDto>> GetListaOrdenTrabajoEstadoAsync(int vigencia, string bearer = null)
        {
            var result = new List<OrdenTrabajoDto>();

            try
            {
                SetBearer(bearer);

                // OJO: plural si tu controller es OrdenesTrabajoController
                var url = "/api/OrdenesTrabajo/ListaOrdenesVigentes?vigencia=" + vigencia;

                Trace.TraceInformation("[OrdenTrabajoService] Llamando a: " + _http.BaseAddress + url);

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return result;

                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceError($"[OrdenTrabajoService.GetListaOrdenTrabajoEstadoAsync] HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}");
                        return result;
                    }

                    var json = await resp.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(json))
                        return result;

                    result = JsonConvert.DeserializeObject<List<OrdenTrabajoDto>>(json)
                             ?? new List<OrdenTrabajoDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[OrdenTrabajoService.GetListaOrdenTrabajoEstadoAsync] " + ex);
            }

            return result;
        }

    }
}
