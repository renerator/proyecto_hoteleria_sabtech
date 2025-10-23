using Front_Hoteleria.Dto.adm.Habitacion;
using Front_Hoteleria.Models.Habitacion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Linq;

namespace Front_Hoteleria.Services.HabitacionInsumo
{
    public class HabitacionInsumoService : IHabitacionInsumoService
    {
        private static readonly HttpClient _http;

        static HabitacionInsumoService()
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

        public async Task<List<InventarioFilaVm>> ListarAsync(int vigencia = 1, string bearer = null)
        {
            var list = new List<InventarioFilaVm>();

            try
            {
                SetBearer(bearer);

                // Interpolación correcta
                var url = "/api/HabitacionInsumo/ListarHabitacionInsumo?vigencia=" + vigencia;
               

                using (var res = await _http.GetAsync(url))
                {
                    if (res.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Trace.TraceWarning("[ListarAsync] 401 Unauthorized desde API HabitacionInsumo.");
                        return list;
                    }

                    if (!res.IsSuccessStatusCode)
                    {
                        Trace.TraceError($"[ListarAsync] Error HTTP ({(int)res.StatusCode}) {res.ReasonPhrase}");
                        return list;
                    }

                    var json = await res.Content.ReadAsStringAsync();
                    var token = JToken.Parse(json);

                    // C# 7.3: evitar ?: con tipos distintos
                    IEnumerable<JToken> items;
                    if (token.Type == JTokenType.Array)
                        items = ((JArray)token).Children();
                    else
                        items = Enumerable.Repeat(token, 1);

                    foreach (var x in items)
                    {
                        list.Add(new InventarioFilaVm
                        {
                            IdHabitacionInsumo = (int?)x["idHabitacionInsumo"] ?? (int?)x["IdHabitacionInsumo"] ?? 0,
                            IdHabitacion = (int?)x["idHabitacion"] ?? (int?)x["IdHabitacion"] ?? 0,
                            IdInsumo = (int?)x["idInsumo"] ?? (int?)x["IdInsumo"] ?? 0,
                            NombreInsumo = (string)(x["nombreInsumo"] ?? x["NombreInsumo"]) ?? "",
                            StockMinimo = (int?)(x["stockMinimo"] ?? x["StockMinimo"]),
                            IdBodega = (int?)(x["idBodega"] ?? x["IdBodega"])
                        });
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError($"[ListarAsync] HttpRequestException: {ex}");
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError($"[ListarAsync] Timeout/TaskCanceled: {ex}");
            }
            catch (JsonException ex)
            {
                Trace.TraceError($"[ListarAsync] JsonException al parsear respuesta: {ex}");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[ListarAsync] Error inesperado: {ex}");
            }

            return list;
        }
    }
}
