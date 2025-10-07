using Newtonsoft.Json;   // Install-Package Newtonsoft.Json
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System;
using System.Configuration;
using System.Configuration;
using System.Net.Http;
using System.Net.Http;
using System.Text;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public PartialViewResult LoginModal()
        {
            return PartialView("_LoginModal");
        }

     
// ...

[HttpPost]
    public async Task<JsonResult> DoLogin(string usuario, string password)
    {
        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            return Json(new { ok = false, message = "Ingrese usuario y contraseña." });

        var apiBase = ConfigurationManager.AppSettings["ApiBaseUrl"];
        var apiLogin = ConfigurationManager.AppSettings["ApiLoginEndpoint"];
        if (string.IsNullOrWhiteSpace(apiBase))
            return Json(new { ok = false, message = "ApiBaseUrl no está configurado." });

        var url = apiBase.TrimEnd('/') + apiLogin;

        using (var client = new HttpClient())
        {
            var payload = new
            {
                id = 0,
                username = usuario,
                password = password
            };

            HttpResponseMessage resp;
            try
            {
                var body = JsonConvert.SerializeObject(payload);
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                resp = await client.PostAsync(url, content);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = "No se pudo contactar la API.", error = ex.Message });
            }

            if (!resp.IsSuccessStatusCode)
                return Json(new { ok = false, message = "Credenciales inválidas o API no disponible." });

            var respText = await resp.Content.ReadAsStringAsync();

            // ---- Parse robusto de la respuesta ----
            string token = null;
            int? idUser = null;
            int? idPerfil = null;

            try
            {
                var jo = JObject.Parse(respText);

                // Token: intentamos varios nombres comunes
                token = (string)(jo["token"] ?? jo["access_token"] ?? jo["jwt"] ?? jo["Token"]);

                // idUser con variantes
                idUser = (int?)(
                    jo["idUser"] ?? jo["IdUser"] ?? jo["idUsuario"] ?? jo["IdUsuario"] ?? jo["userId"] ?? jo["UserId"]
                );

                // idPerfil con variantes
                idPerfil = (int?)(
                    jo["idPerfil"] ?? jo["IdPerfil"] ?? jo["perfilId"] ?? jo["PerfilId"] ?? jo["roleId"] ?? jo["RoleId"]
                );
            }
            catch
            {
                return Json(new { ok = false, message = "Respuesta de login no válida." });
            }

            if (string.IsNullOrWhiteSpace(token))
                return Json(new { ok = false, message = "La API no devolvió token." });

            // ---- Guardar en Session (opcional) ----
            Session["Token"] = token;
            Session["Usuario"] = usuario;
            if (idUser.HasValue) Session["IdUsuario"] = idUser.Value;
            if (idPerfil.HasValue) Session["IdPerfil"] = idPerfil.Value;

            // ---- Guardar en cookies (1 hora) ----
            // Cookie del token
            var tokenCookie = new HttpCookie("AuthToken", token)
            {
                HttpOnly = true,
                Secure = Request.IsSecureConnection,
                Expires = DateTime.Now.AddHours(1),
                Path = "/"
            };
            // Si tu framework lo soporta:
            try { tokenCookie.SameSite = (SameSiteMode)Enum.Parse(typeof(SameSiteMode), "Lax"); } catch { /* ignora si no existe */ }
            Response.Cookies.Add(tokenCookie);

            // Cookie IdUser
            if (idUser.HasValue)
            {
                var userCookie = new HttpCookie("AuthUserId", idUser.Value.ToString())
                {
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection,
                    Expires = DateTime.Now.AddHours(1),
                    Path = "/"
                };
                try { userCookie.SameSite = (SameSiteMode)Enum.Parse(typeof(SameSiteMode), "Lax"); } catch { }
                Response.Cookies.Add(userCookie);
            }

            // Cookie IdPerfil
            if (idPerfil.HasValue)
            {
                var perfilCookie = new HttpCookie("AuthPerfilId", idPerfil.Value.ToString())
                {
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection,
                    Expires = DateTime.Now.AddHours(1),
                    Path = "/"
                };
                try { perfilCookie.SameSite = (SameSiteMode)Enum.Parse(typeof(SameSiteMode), "Lax"); } catch { }
                Response.Cookies.Add(perfilCookie);
            }

            // Puedes devolver también idUser/idPerfil para el cliente si los necesita
            return Json(new
            {
                ok = true,
                token,
                idUser = idUser ?? 0,
                idPerfil = idPerfil ?? 0
            });
        }
    }
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult LogoutPost()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}
