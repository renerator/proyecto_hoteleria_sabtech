using Front_Hoteleria.Dto.Huesped;
using Front_Hoteleria.Services.ReclamosHuesped;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace Front_Hoteleria.Controllers
{
    public class ReclamosHuespedController : Controller
    {
        private readonly IReclamosHuespedService _api;

        public ReclamosHuespedController() : this(new ReclamosHuespedService())
        {
        }

        public ReclamosHuespedController(IReclamosHuespedService api)
        {
            _api = api;
        }

        // =========================
        // TOKEN
        // =========================
        private string GetBearer()
        {
            try
            {
                return (Session["Token"] as string)
                       ?? (Request.Cookies["access_token"] != null
                           ? Request.Cookies["access_token"].Value
                           : null);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[GetBearer] Error leyendo token: {ex}");
                return null;
            }
        }

        // =========================
        // INDEX (elige vista según perfil)
        // =========================
        [HttpGet]
        public ActionResult Index()
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            var perfilObj = Session["IdPerfil"];
            if (perfilObj == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            var perfil = Convert.ToInt32(perfilObj);

            switch (perfil)
            {
                case 1: // admin
                    return View("~/Views/Reservas/Index.cshtml");
                case 2: // huésped
                    return View("~/Views/Huesped/Reclamos/Index.cshtml");
                default:
                    return RedirectToAction("Login", "Account");
            }
        }

        // Alias por si llamas /ReclamosHuesped/Reclamos
        [HttpGet]
        public ActionResult Reclamos()
        {
            return RedirectToAction("Index");
        }

        // =========================
        // TABLA RECLAMOS (PARCIAL, PARA AJAX)
        // =========================
        [HttpGet]
        public async Task<ActionResult> TablaReclamos()
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

                var lista = await _api.ListarReclamosHuespedAsync(token)
                            ?? new List<ReclamoSolicitudDto>();

                return PartialView("~/Views/Huesped/Reclamos/_TablaReclamos.cshtml", lista);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[TablaReclamos] " + ex);
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Error al cargar los reclamos.");
            }
        }

        // =========================
        // CREAR RECLAMO / SUGERENCIA (USA IDs DE LOS COMBOS)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear()
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { ok = false, message = "Sesión expirada." });

                var dto = MapearDesdeForm(Request);

                // Validaciones básicas usando IDs
                if (dto.IdTipoSolicitudHuesped <= 0 ||
                    dto.IdCategoriaHuesped <= 0 ||
                    dto.IdPrioridad <= 0 ||
                    string.IsNullOrWhiteSpace(dto.Asunto) ||
                    string.IsNullOrWhiteSpace(dto.Descripcion) ||
                    string.IsNullOrWhiteSpace(dto.Email))
                {
                    return Json(new { ok = false, message = "Debe completar todos los campos obligatorios." });
                }

                var ok = await _api.CrearReclamoHuespedAsync(dto, token);

                return Json(new
                {
                    ok,
                    message = ok
                        ? "Solicitud registrada correctamente."
                        : "No se pudo registrar la solicitud."
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ReclamosHuespedController.Crear] " + ex);
                return Json(new { ok = false, message = "Ocurrió un error al registrar la solicitud." });
            }
        }
        
// ...

[HttpGet]
    public async Task<ActionResult> Detalle(int idReclamoHuesped)
    {
        try
        {
            var token = GetBearer();
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada.");

            if (idReclamoHuesped <= 0)
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, "Id inválido.");

            var dto = await _api.ObtenerReclamoHuespedPorIdAsync(idReclamoHuesped, token);
            if (dto == null)
                return HttpNotFound("No se encontró la solicitud.");

            return PartialView("~/Views/Huesped/Reclamos/_DetalleReclamo.cshtml", dto);
        }
        catch (Exception ex)
        {
            Trace.TraceError("[ReclamosHuesped.Detalle] " + ex);
            return new HttpStatusCodeResult(
                (int)HttpStatusCode.InternalServerError,
                "Error al cargar el detalle de la solicitud.");
        }
    }

    // =========================
    // MAPEOS AUXILIARES
    // =========================
    private ReclamoSolicitudDto MapearDesdeForm(HttpRequestBase request)
        {
            // IDs que vienen de los combos
            int.TryParse(request["idTipoSolicitudHuesped"], out var idTipoSolicitudHuesped);
            int.TryParse(request["idCategoriaSolicitudHuesped"], out var idCategoriaHuesped);

            // Prioridad viene como texto: normal / alta / urgente
            var prioridadRaw = (request["Prioridad"] ?? "").ToLowerInvariant();
            var idPrioridad = MapPrioridadId(prioridadRaw);
            var prioridadTxt = MapPrioridad(prioridadRaw);

            // Usuario actualizador (si tienes el id en sesión)
            int idUsuario = 0;
            if (Session["IdUsuario"] != null)
                int.TryParse(Session["IdUsuario"].ToString(), out idUsuario);
            else if (Session["IdTrabajador"] != null)
                int.TryParse(Session["IdTrabajador"].ToString(), out idUsuario);

            return new ReclamoSolicitudDto
            {
                idReclamoHuesped = 0, // lo genera el backend

                IdTipoSolicitudHuesped = idTipoSolicitudHuesped,
                TipoSolicitud = MapTipoSolicitud(idTipoSolicitudHuesped),

                IdCategoriaHuesped = idCategoriaHuesped,
                Categoria = MapCategoria(idCategoriaHuesped),

                Asunto = request["Asunto"],
                Descripcion = request["Descripcion"],
                Email = request["EmailHuesped"],

                IdPrioridad = idPrioridad,
                Prioridad = prioridadTxt,

                Fecha = DateTime.Now,

                IdEstado = 1,              // 1 = Pendiente (hot_EstadoHuesped)
                Estado = "Pendiente",

                Respuesta = null,
                FechaRespuesta = null,

                IdUsuarioActualizacion = idUsuario
            };
        }

        private string MapTipoSolicitud(int id)
        {
            switch (id)
            {
                case 1: return "Reclamo";
                case 2: return "Sugerencia";
                case 3: return "Felicitación";
                case 4: return "Queja";
                default: return "";
            }
        }

        private string MapCategoria(int id)
        {
            switch (id)
            {
                case 1: return "Habitación";
                case 2: return "Servicios";
                case 3: return "Comida";
                case 4: return "Limpieza";
                case 5: return "Personal";
                case 6: return "Instalaciones";
                case 7: return "Otros";
                default: return "";
            }
        }

        private int MapPrioridadId(string prioridadRaw)
        {
            switch (prioridadRaw)
            {
                case "normal": return 1;   // Id 1 en hot_PrioridadHuesped
                case "alta": return 2;   // Id 2
                case "urgente": return 3;   // Id 3
                default: return 0;
            }
        }

        private string MapPrioridad(string prioridadRaw)
        {
            switch (prioridadRaw)
            {
                case "normal": return "Normal";
                case "alta": return "Alta";
                case "urgente": return "Urgente";
                default: return "";
            }
        }
    }
}
