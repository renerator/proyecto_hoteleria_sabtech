using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;
using System.Web;
using Front_Hoteleria.Models;

namespace Front_Hoteleria.Services
{
    public class MenuService
    {
        private readonly HttpClient _http;

        public MenuService()
        {
            _http = new HttpClient();
        }

        public async Task<List<MenuItem>> ObtenerMenuAsync(int idUsuario, int idPerfil)
        {
            var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            var endpoint = ConfigurationManager.AppSettings["ApiMenuEndpoint"];
            var url = $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}?IdUsuario={idUsuario}&IdPerfil={idPerfil}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Leer token de la sesión
            var session = HttpContext.Current?.Session;
            var token = session != null ? session["Token"] as string : null;

            
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _http.SendAsync(request).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return new List<MenuItem>();
            }

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var funcionalidades = JsonConvert.DeserializeObject<List<FuncionalidadDto>>(json)
                                  ?? new List<FuncionalidadDto>();

            // 1
            var menus = funcionalidades.Where(f => f.esMenu == 1).ToList();
            var hijos = funcionalidades.Where(f => f.esMenu == 0).ToList();

            var resultado = new List<MenuItem>();

            
            foreach (var menu in menus)
            {
                var padre = new MenuItem
                {
                    Titulo = menu.descripcion,
                    Icono = MapIcono(menu.descripcion)
                };

              
                var subItems = hijos
                    .Where(h => h.idTipoFuncionalidad == menu.idTipoFuncionalidad)
                    .OrderBy(h => h.descripcion);

                foreach (var hijo in subItems)
                {
                    padre.SubMenu.Add(new MenuItem
                    {
                        Titulo = hijo.descripcion,
                        Url = hijo.pagina
                    });
                }

                resultado.Add(padre);
            }

            return resultado;
        }



        private string MapIcono(string nombreGrupo)
        {
            var n = (nombreGrupo ?? "").ToLowerInvariant();
            if (n.Contains("admin")) return "fa fa-check";
            if (n.Contains("manten")) return "fa fa-archive";
            if (n.Contains("huésped") || n.Contains("huesped")) return "fa fa-check-square";
            return "fa fa-folder";
        }
    }
}
