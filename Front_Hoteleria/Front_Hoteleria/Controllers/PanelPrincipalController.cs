using Front_Hoteleria.Model.Reserva;
using Front_Hoteleria.Services.PanelPrincipal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;


namespace Front_Hoteleria.Controllers
{
   

    public class PanelPrincipalController : Controller
    {
        private readonly IPanelPrincipalService _api;

        public PanelPrincipalController() : this(new PanelPrincipalService()) { }
        public PanelPrincipalController (IPanelPrincipalService api) { _api = api; }

        // -------------------------------
        //  TOKEN & PERFIL
        // -------------------------------
        private string GetBearer()
        {
            try
            {
                return (Session["Token"] as string)
                       ?? (Request.Cookies["access_token"] != null ? Request.Cookies["access_token"].Value : null);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[GetBearer] Error leyendo token: {ex}");
                return null;
            }
        }


        [HttpGet]
        public ActionResult Index()
        {

            if (!(Session["Token"] is string tok) || string.IsNullOrWhiteSpace(tok))
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            var perfil = Session["IdPerfil"];
            if (perfil == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });


            return View("~/Views/PanelPrincipal/Index.cshtml");
            
        }

        // Parcial: tarjetas + gráfico (maqueta)
        [HttpGet, Route("Dashboard")]
        public PartialViewResult Dashboard()
        {
            return PartialView("~/Views/PanelPrincipal/_DashboardAdm.cshtml");
        }
        [HttpGet, Route("Tabla")]
        public async Task<ActionResult> TablaPartial(
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int? idEstadoReserva,
            int? idtiporeserva)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult(401, "Sesión expirada");

                var filtro = new ReservaTrabajadorModel
                {
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta,
                    IdEstadoReserva = idEstadoReserva ?? 0, // 0 = no filtra (según tu SP)
                    IdTipoReserva = idtiporeserva ?? 0
                };

                var data = await _api.ReservasDisponiblesTrabajadorAsync(filtro, token);

                // TIP: el partial debe estar tipado a List<ReservaTrabajadorModel>
                return PartialView("~/Views/PanelPrincipal/_UpsertDashAdm.cshtml", data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"[TablaReserva] {ex}");
                return new HttpStatusCodeResult(500, "Error al cargar reservas");
            }
        }
        // Parcial: tabla de reservas (maqueta)
        

        // Parcial: cuerpo de modal "Nueva Reserva" (maqueta)
        [HttpGet, Route("Upsert")]
        public PartialViewResult Upsert()
        {
            return PartialView("~/Views/PanelPrincipal/_UpsertDashAdm.cshtml");
        }
    }
}

