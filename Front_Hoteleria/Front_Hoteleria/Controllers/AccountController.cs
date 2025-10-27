using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class AccountController : Controller
    {
        // ===== Helpers de sesión/perfil =====
        private int? GetIdPerfil()
        {
            var raw = Session["IdPerfil"];
            if (raw == null) return null;
            if (raw is int i) return i;

            int parsed;
            return int.TryParse(raw.ToString(), out parsed) ? (int?)parsed : null;
        }

        private string GetRedirectUrlByProfile(int? idPerfil)
        {
            switch (idPerfil)
            {
                case 1: return Url.Action("Index", "Reservas");          // Admin
                case 2: return Url.Action("Index", "ReservasHuesped");   // Huésped
                case 3: return Url.Action("Index", "ServiciosPersonal");         // Personal
                case 4: return Url.Action("Index", "HabitacionesPlataforma");      // Plataforma
                default: return Url.Action("Index", "Home");
            }
        }

        // ===== GET Login =====
        [HttpGet]
        public ActionResult Login(string returnUrl = null)
        {
            // Si ya estás autenticado, deriva según perfil o respeta returnUrl local
            var token = Session["Token"] as string;
            if (!string.IsNullOrWhiteSpace(token))
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                var idPerfil = GetIdPerfil();
                return Redirect(GetRedirectUrlByProfile(idPerfil));
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // ===== POST Login (no Ajax) =====
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string usuario, string password, bool? recordar, string returnUrl = null)
        {
            var result = await AutenticarContraApi(usuario, password);
            if (!result.Ok)
            {
                ViewBag.Error = result.Message ?? "No se pudo iniciar sesión.";
                ViewBag.ReturnUrl = returnUrl;
                return View(); // re-muestra el login con error
            }

            // Guardar sesión y cookies
            GuardarSesionYCookies(result.Token, usuario, result.IdUser, result.IdPerfil, recordar == true);

            // Redirigir
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return Redirect(GetRedirectUrlByProfile(result.IdPerfil));
        }

        // ===== POST Login Ajax =====
        [HttpPost]
        // [ValidateAntiForgeryToken] // Si usas antiforgery vía Ajax, habilítalo
        public async Task<JsonResult> DoLogin(string usuario, string password, bool? recordar, string returnUrl = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
                    return Json(new { ok = false, message = "Ingrese usuario y contraseña." });

                usuario = usuario.Trim();
                password = password.Trim();

                var result = await AutenticarContraApi(usuario, password);
                if (!result.Ok)
                    return Json(new { ok = false, message = result.Message });

                // Guarda sesión y cookies
                GuardarSesionYCookies(result.Token, usuario, result.IdUser, result.IdPerfil, recordar == true);

                var redirect = (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                               ? returnUrl
                               : GetRedirectUrlByProfile(result.IdPerfil);

                return Json(new { ok = true, redirect });
            }
            catch (OperationCanceledException oce)
            {
                Trace.TraceError($"[DoLogin] Timeout/operación cancelada: {oce}");
                Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                return Json(new { ok = false, message = "La solicitud tardó demasiado. Intente nuevamente." });
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[DoLogin] Error inesperado: {ex}");
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { ok = false, message = "Ocurrió un error al iniciar sesión. Intente nuevamente." });
            }
        }

        // ===== Logout =====
        [HttpGet]
        public ActionResult Logout()
        {
            LimpiarSesionYCookies();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public ActionResult LogoutPost()
        {
            LimpiarSesionYCookies();
            return RedirectToAction("Login", "Account");
        }

        // ===== Comunicación con API de autenticación =====
        private async Task<(bool Ok, string Message, string Token, int IdUser, int IdPerfil)> AutenticarContraApi(string usuario, string password)
        {
            var apiBase = ConfigurationManager.AppSettings["ApiBaseUrl"];
            var apiLogin = ConfigurationManager.AppSettings["ApiLoginEndpoint"]; // p.ej. "/api/v1/Autenticacion/Login"
            if (string.IsNullOrWhiteSpace(apiBase))
                return (false, "ApiBaseUrl no está configurado.", null, 0, 0);

            var url = apiBase.TrimEnd('/') + apiLogin;

            using (var client = new HttpClient())
            {
                try
                {
                    var payload = new { id = 0, username = usuario, password = password };
                    var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                    var resp = await client.PostAsync(url, content);
                    if (!resp.IsSuccessStatusCode)
                        return (false, "Credenciales inválidas o API no disponible.", null, 0, 0);

                    var respText = await resp.Content.ReadAsStringAsync();

                    // Parse robusto
                    var jo = JObject.Parse(respText);
                    var token = (string)(jo["token"] ?? jo["access_token"] ?? jo["jwt"] ?? jo["Token"]);
                    if (string.IsNullOrWhiteSpace(token))
                        return (false, "La API no devolvió token.", null, 0, 0);

                    int idUser = (int?)(
                        jo["idUser"] ?? jo["IdUser"] ?? jo["idUsuario"] ?? jo["IdUsuario"] ?? jo["userId"] ?? jo["UserId"]
                    ) ?? 0;

                    int idPerfil = (int?)(
                        jo["idPerfil"] ?? jo["IdPerfil"] ?? jo["perfilId"] ?? jo["PerfilId"] ?? jo["roleId"] ?? jo["RoleId"]
                    ) ?? 0;

                    return (true, null, token, idUser, idPerfil);
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"[AutenticarContraApi] Error: {ex}");
                    return (false, "No se pudo contactar la API.", null, 0, 0);
                }
            }
        }

        // ===== Manejo de sesión/cookies =====
        private void GuardarSesionYCookies(string token, string usuario, int idUser, int idPerfil, bool recordar)
        {
            // Sesión
            Session["Token"] = token;
            Session["Usuario"] = usuario;
            Session["IdUsuario"] = idUser;
            Session["IdPerfil"] = idPerfil;

            // Cookies (1 hora; si 'recordar', 7 días)
            var exp = DateTime.Now.AddHours(1);
            if (recordar) exp = DateTime.Now.AddDays(7);

            AgregarCookie("AuthToken", token, exp, httpOnly: true);
            if (idUser > 0) AgregarCookie("AuthUserId", idUser.ToString(), exp, httpOnly: true);
            if (idPerfil > 0) AgregarCookie("AuthPerfilId", idPerfil.ToString(), exp, httpOnly: true);
        }

        private void AgregarCookie(string name, string value, DateTime exp, bool httpOnly)
        {
            var c = new HttpCookie(name, value)
            {
                HttpOnly = httpOnly,
                Secure = Request.IsSecureConnection,
                Expires = exp,
                Path = "/"
            };
            try { c.SameSite = (SameSiteMode)Enum.Parse(typeof(SameSiteMode), "Lax"); } catch { }
            Response.Cookies.Add(c);
        }

        private void LimpiarSesionYCookies()
        {
            Session.Clear();

            void Borrar(string n)
            {
                var c = new HttpCookie(n, "")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Path = "/"
                };
                Response.Cookies.Add(c);
            }

            Borrar("AuthToken");
            Borrar("AuthUserId");
            Borrar("AuthPerfilId");
        }
    }
}
