using Front_Hoteleria.Dto.Empresa;
// using Front_Hoteleria.Services.Empresa; // <- quitar esta línea
using Front_Hoteleria.Services.Empresa;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Empresa
{
    public class EmpresaService : IEmpresaService
    {
        private static readonly HttpClient _http;

        static EmpresaService()
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

        // GET /api/Empresas/combo?soloActivas=true&filtro=abc
        public async Task<List<EmpresaDto>> ListarComboAsync(
            bool? soloActivas = true,
            string filtro = null,
            string bearer = null)
        {
            try
            {
                SetBearer(bearer);

                var qs = new List<string>();
                if (soloActivas.HasValue)
                    qs.Add("soloActivas=" + (soloActivas.Value ? "true" : "false"));
                if (!string.IsNullOrWhiteSpace(filtro))
                    qs.Add("filtro=" + Uri.EscapeDataString(filtro));

                var url = "/api/Empresa/combo";
                if (qs.Count > 0)
                    url += "?" + string.Join("&", qs);

                using (var resp = await _http.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        return new List<EmpresaDto>();

                    if (!resp.IsSuccessStatusCode)
                    {
                        var err = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[EmpresasService.ListarComboAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {err}");
                        return new List<EmpresaDto>();
                    }

                    var json = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<EmpresaDto>>(json)
                           ?? new List<EmpresaDto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[EmpresasService.ListarComboAsync] " + ex);
                return new List<EmpresaDto>();
            }
        }


        // ——— NUEVO: Crear empresa ———
        public async Task<bool> CrearAsync(EmpresaCrearPostDto dto, string bearer = null)
        {
            try
            {
                if (dto == null) return false;

                // Defaults por si no vienen desde el modal
                dto.Estado = true;
                dto.IdPais = null;
               // dto.IdEmpresa = null;

                SetBearer(bearer);

                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // ¡OJO! Endpoint singular según tu imagen: /api/Empresa/crear
                using (var resp = await _http.PostAsync("/api/Empresa/crear", content))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var body = await resp.Content.ReadAsStringAsync();
                        Trace.TraceWarning($"[EmpresaService.CrearAsync] {(int)resp.StatusCode} {resp.ReasonPhrase} -> {body}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[EmpresaService.CrearAsync] " + ex);
                return false;
            }
        }
        }
}
