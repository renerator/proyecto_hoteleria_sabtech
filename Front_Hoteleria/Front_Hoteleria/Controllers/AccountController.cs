using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;   // Install-Package Newtonsoft.Json
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Front_Hoteleria.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public PartialViewResult LoginModal()
        {
            return PartialView("_LoginModal");
        }

        // 1) Usuario ingresa usuario y password en el modal
        // 2) Validamos contra la API y obtenemos el token
        // 3) Guardamos token y usuario en Session
        [HttpPost]
        public async Task<JsonResult> DoLogin(string usuario, string password)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
                return Json(new { ok = false, message = "Ingrese usuario y contraseña." });



            var apiBase = ConfigurationManager.AppSettings["ApiBaseUrl"]; // ej: https://localhost:44393/api/
            var apiLogin = ConfigurationManager.AppSettings["ApiLoginEndpoint"]; // ej: https://localhost:44393/api/
            if (string.IsNullOrWhiteSpace(apiBase))
                return Json(new { ok = false, message = "ApiBaseUrl no está configurado." });

            var url = apiBase.TrimEnd('/') + apiLogin;

            using (var client = new HttpClient())
            {
                // ⚠️ AJUSTA los nombres EXACTOS que espera tu API
                var payload = new
                {
                    id = 0,                 // si tu API requiere id, enviamos 0
                    username = usuario,     // mapeo desde tu textbox "usuario"
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

                // Esperamos una respuesta que contenga el token (p.ej.: { "token":"..." })
                string token;
                try
                {
                    dynamic data = JsonConvert.DeserializeObject(respText);
                    token = (string)(data?.token ?? "");
                }
                catch
                {
                    return Json(new { ok = false, message = "Respuesta de login no válida." });
                }

                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { ok = false, message = "La API no devolvió token." });

                // Guardar en Session para usar en todas las páginas
                Session["JWT"] = token;
                Session["Usuario"] = usuario;

                return Json(new { ok = true, token });
            }
        }

        [HttpPost]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
