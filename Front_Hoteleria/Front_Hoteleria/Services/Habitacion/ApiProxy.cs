using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Api
{
    public class ApiProxy
    {
        private static readonly HttpClient _http;

        static ApiProxy()
        {
            var baseUrl = ConfigurationManager.AppSettings["Api.BaseUrl"]
              ?? ConfigurationManager.AppSettings["ApiBaseUrl"];
            _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static void SetBearer(string bearer)
        {
            _http.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrWhiteSpace(bearer))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        public static async Task<string> GetStringAsync(string path, string bearer = null)
        {
            SetBearer(bearer);
            using (var resp = await _http.GetAsync(path))
            {
                resp.EnsureSuccessStatusCode();
                return await resp.Content.ReadAsStringAsync();
            }
        }
    }
}