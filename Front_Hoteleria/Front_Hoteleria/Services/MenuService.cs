using Front_Hoteleria.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace Front_Hoteleria.Services
{
    public class MenuService
    {
        private readonly HttpClient _http;

        public MenuService()
        {
            _http = new HttpClient();
        }

        public async Task<List<MenuDTO>> ObtenerMenuAsync(int idUsuario, int idPerfil)
        {
            var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            var endpoint = ConfigurationManager.AppSettings["ApiMenuEndpoint"];
            var url = $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}?IdUsuario={idUsuario}&IdPerfil={idPerfil}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Leer token de la sesión
            var token = HttpContext.Current?.Session?["Token"] as string;
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                return new List<MenuDTO>();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var funcionalidades = JsonConvert.DeserializeObject<List<FuncionalidadDto>>(json)
                                  ?? new List<FuncionalidadDto>();

            // separar menús y hojas
            var menus = funcionalidades.Where(f => f.esMenu == 1).ToList();
            var hijos = funcionalidades.Where(f => f.esMenu == 0).ToList();

            var resultado = new List<MenuDTO>();

            // 🟡 Caso especial: perfil huésped sin menús
            if (menus.Count == 0)
            {
                // crea un contenedor “Huésped”
                var padre = new MenuDTO
                {
                    Titulo = "Huésped",
                    Icono = "fa fa-user"
                };

                // todos los hijos del perfil huésped
                var subItems = hijos
                    .Where(h => string.Equals(h.Perfil, "Huesped", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(h => h.descripcion);

                foreach (var hijo in subItems)
                {
                    padre.SubMenu.Add(new MenuDTO
                    {
                        Titulo = hijo.descripcion,
                        Url = hijo.pagina,
                        Icono = MapIcono(hijo.descripcion)
                    });
                }

                // si hay algo que mostrar
                if (padre.SubMenu.Count > 0)
                    resultado.Add(padre);
            }
            else
            {
                // 🟢 Caso normal: perfil con menús definidos
                foreach (var menu in menus)
                {
                    var padre = new MenuDTO
                    {
                        Titulo = menu.descripcion,
                        Icono = MapIcono(menu.descripcion)
                    };

                    var subItems = hijos
                        .Where(h => h.idPadre == menu.idPadre) // relación correcta padre→hijo
                        .OrderBy(h => h.descripcion);

                    foreach (var hijo in subItems)
                    {
                        padre.SubMenu.Add(new MenuDTO
                        {
                            Titulo = hijo.descripcion,
                            Url = hijo.pagina,
                            Icono = MapIcono(hijo.descripcion)
                        });
                    }

                    // agrega solo si tiene hijos
                    //if (padre.SubMenu.Count > 0)
                        resultado.Add(padre);
                }
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
