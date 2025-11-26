using Front_Hoteleria.Dto.Habitacion;
using Front_Hoteleria.Dto.Inventario;
using Front_Hoteleria.Dtos.Habitacion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static System.Net.WebRequestMethods;

namespace Front_Hoteleria.Services.HabitacionInventario
{
    public class HabitacionInventarioService : IHabitacionInventarioService
    {
        private static readonly HttpClient _http;

        static HabitacionInventarioService()
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


public async Task<List<InventarioHabitacionDTO>> ListarAsync(int vigencia = 1, string bearer = null)
    {
        var list = new List<InventarioHabitacionDTO>();

        try
        {
            SetBearer(bearer);

            var url = "/api/HabitacionInventario/ListarHabitacionInsumo?vigencia=" + vigencia;

            using (var res = await _http.GetAsync(url).ConfigureAwait(false))
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

                var json = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(json))
                    return list;

                var token = JToken.Parse(json);

                // 1) Respuesta es un array puro:  [ { ... }, { ... } ]
                if (token.Type == JTokenType.Array)
                {
                    list = token.ToObject<List<InventarioHabitacionDTO>>() ?? new List<InventarioHabitacionDTO>();
                }
                // 2) Respuesta envuelta: { data: [ ... ] } o similar
                else if (token.Type == JTokenType.Object)
                {
                    var obj = (JObject)token;
                    var data = obj["data"] ?? obj["Data"] ?? obj["resultado"] ?? obj["Resultado"];

                    if (data != null && data.Type == JTokenType.Array)
                    {
                        list = data.ToObject<List<InventarioHabitacionDTO>>() ?? new List<InventarioHabitacionDTO>();
                    }
                    else
                    {
                        // 3) Un solo ítem: { ... }
                        var single = obj.ToObject<InventarioHabitacionDTO>();
                        if (single != null)
                            list.Add(single);
                    }
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
