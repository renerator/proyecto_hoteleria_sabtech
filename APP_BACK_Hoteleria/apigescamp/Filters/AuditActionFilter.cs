using DemoBackend.Models.Auditoria;
using DemoBackend.RepositoryGes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DemoBackend.Filters
{
    public class AuditActionFilter : IAsyncActionFilter
    {
        private readonly IGenericRepositoryEntity<AuditoriaModel> _listaAuditoria;

        public AuditActionFilter(IGenericRepositoryEntity<AuditoriaModel> listaAuditoria)
        {
            _listaAuditoria = listaAuditoria;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var http = context.HttpContext;
            var request = http.Request;

            // Ejecuta la acción primero (no interferir)
            var executed = await next();

            // ===== Modulo limpio: "Controller.Action"
            var cad = context.ActionDescriptor as ControllerActionDescriptor;
            string modulo = cad != null
                ? $"{cad.ControllerName}.{cad.ActionName}"
                : (context.ActionDescriptor.DisplayName?.Split('(')[0].Trim() ?? string.Empty);

            // ===== TablaAfectada exacta (mapeada a tus nombres de BD)
            string tablaAfectada = MapTablaExacta(
                cad?.ControllerName,
                cad?.ActionName,
                request.Path
            );

            DateTime fechaAccion = DateTime.Now;

            // ===== idUsuario para TODOS los endpoints
            bool esLogin = IsLogin(cad, request);

            // 1) Login: prioriza id puesto por el controlador
            int? idUsuario = null;
            if (esLogin && http.Items.TryGetValue("idUsuarioLogin", out var idObj) && idObj is int idFromItems)
            {
                idUsuario = idFromItems;
            }

            // 2) Claims del JWT (para endpoints autenticados)
            if (!idUsuario.HasValue)
                idUsuario = GetUserIdFromClaims(http.User);

            // 3) (opcional) Fallback: route/query/body
            if (!idUsuario.HasValue)
                idUsuario = ResolveFromRouteQueryOrBody(context, http);

            // Login: fuerza nombre exacto que necesitas
            if (esLogin)
                tablaAfectada = "ctr_usuario  sp:ctr_credenciales_msctr";

            // ===== Insert por SP (tu patrón InsertProcedure)
            try
            {
                const string sql = "AUD_INS_Log @idUsuario,@Accion,@Modulo,@FechaAccion,@TablaAfectada";
                var parametros = new[]
                {
                    new SqlParameter("@idUsuario", (object?)idUsuario ?? DBNull.Value),
                    new SqlParameter("@Accion", request.Method),
                    new SqlParameter("@Modulo", modulo),
                    new SqlParameter("@FechaAccion", fechaAccion),
                    new SqlParameter("@TablaAfectada", tablaAfectada)
                };

                _listaAuditoria.InsertProcedure(sql, parametros);
            }
            catch (Exception ex)
            {
                // No romper la request por fallo de auditoría
                Console.WriteLine($"[AUDIT_LOG] Error al insertar auditoría: {ex.Message}");
            }
        }

        // ===== Helpers =====

        private static bool IsLogin(ControllerActionDescriptor? cad, HttpRequest request)
        {
            if (request.Path.Value?.IndexOf("login", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return string.Equals(cad?.ControllerName, "Autenticacion", StringComparison.OrdinalIgnoreCase)
                && string.Equals(cad?.ActionName, "Login", StringComparison.OrdinalIgnoreCase);
        }

        private static int? GetUserIdFromClaims(ClaimsPrincipal user)
        {
            // Orden: UniqueName (tu JWT actual) -> "idUsuario" (custom recomendado) -> NameIdentifier
            var v =
                user.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value
                ?? user.FindFirst("idUsuario")?.Value
                ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(v, out var id) ? id : (int?)null;
        }

        private static int? ResolveFromRouteQueryOrBody(ActionExecutingContext ctx, HttpContext http)
        {
            // Route values
            int? routeId = TryParseInt(
          (ctx.RouteData.Values.TryGetValue("idUsuario", out var v1) ? v1?.ToString() : null)
          ?? (ctx.RouteData.Values.TryGetValue("idUser", out var v2) ? v2?.ToString() : null)
          ?? (ctx.RouteData.Values.TryGetValue("userId", out var v3) ? v3?.ToString() : null)
         );

            if (routeId.HasValue) return routeId;

            // Query string
            int? queryId = TryParseInt(
                http.Request.Query["idUsuario"].FirstOrDefault()
                ?? http.Request.Query["idUser"].FirstOrDefault()
                ?? http.Request.Query["userId"].FirstOrDefault()
            );
            if (queryId.HasValue) return queryId;

            // Body (solo si es POST/PUT/PATCH)
            if (http.Request.ContentLength > 0 &&
                (http.Request.Method == HttpMethods.Post || http.Request.Method == HttpMethods.Put || http.Request.Method == HttpMethods.Patch))
            {
                try
                {
                    http.Request.EnableBuffering();
                    using var reader = new StreamReader(http.Request.Body, Encoding.UTF8, leaveOpen: true);
                    var body = reader.ReadToEnd();
                    http.Request.Body.Position = 0;

                    using var doc = JsonDocument.Parse(body);
                    if (doc.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        if (doc.RootElement.TryGetProperty("idUsuario", out var p) && p.TryGetInt32(out var id)) return id;
                        if (doc.RootElement.TryGetProperty("IdUsuario", out p) && p.TryGetInt32(out id)) return id;
                        if (doc.RootElement.TryGetProperty("idUser", out p) && p.TryGetInt32(out id)) return id;
                        if (doc.RootElement.TryGetProperty("userId", out p) && p.TryGetInt32(out id)) return id;
                    }
                }
                catch { /* ignorar parseos fallidos */ }
            }

            return null;
        }

        private static int? TryParseInt(string? s) => int.TryParse(s, out var x) ? x : (int?)null;

        private static string MapTablaExacta(string? controller, string? action, PathString path)
        {
            if (path.Value?.IndexOf("login", StringComparison.OrdinalIgnoreCase) >= 0)
                return "ctr_usuario  sp:ctr_credenciales_msctr";

            var key = (controller ?? "").Trim().ToLowerInvariant();
            return key switch
            {
                "bodegas" => "ctr_man_Bodegas",
                "mantenedores" => "ctr_man_Areas",
                "habitacion" => "ctr_man_habitaciones",
                "habitaciones" => "ctr_man_habitaciones",
                "insumos" => "ctr_man_Insumos",
                "servicios" => "ctr_man_Servicios",
                "reservas" => "hot_Reservas",
                "usuario" => "ctr_usuario",
                "trabajador" => "ctr_usuario",
                _ => controller ?? "Desconocido"
            };
        }
    }
}

