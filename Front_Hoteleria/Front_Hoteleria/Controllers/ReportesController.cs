using Front_Hoteleria.Dto.Reportes;
using Front_Hoteleria.Services.Reportes;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

public class ReportesController : Controller
{
    private readonly IReportesService _api;

    public ReportesController() : this(new ReportesService()) { }

    public ReportesController(IReportesService api)
    {
        _api = api;
    }

    private string GetBearer()
    {
        return (Session["Token"] as string)
               ?? (Request.Cookies["access_token"] != null ? Request.Cookies["access_token"].Value : null);
    }

    // ========== INDEX ==========
    [HttpGet]
    public ActionResult Index()
    {
        return View();
    }

    // ========== PARCIAL KPI ==========
    [HttpGet]
    public async Task<ActionResult> _DashboardReportes()
    {
        var token = GetBearer();
        if (string.IsNullOrWhiteSpace(token))
            return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized);

        var kpi = await _api.ObtenerKpiAsync(token) ?? new ReportesKpiDto();
        return PartialView("_DashboardReportes", kpi);
    }

    // ========== PARCIAL RESTO ==========
    [HttpGet]
    public ActionResult _ReportesOperativos()
    {
        // lo inicializamos vacío; el JS luego llama a los POST para llenar
        var dto = new ReportesOperativoDto();
        return PartialView("_ReportesOperativos", dto);
    }

    // ====== acciones ajax para llenar el dto operativo ======

    [HttpPost]
    public async Task<JsonResult> GenerarCierreTurno(DateTime fecha, string turno)
    {
        var token = GetBearer();
        if (string.IsNullOrWhiteSpace(token))
            return Json(new { ok = false, message = "Sesión expirada." });

        var data = await _api.GenerarCierreTurnoAsync(fecha, turno, token);
        if (data == null)
            return Json(new { ok = false, message = "No se pudo generar el cierre." });

        return Json(new { ok = true, data });
    }

    [HttpPost]
    public async Task<JsonResult> GenerarReporteDiario(DateTime fecha)
    {
        var token = GetBearer();
        if (string.IsNullOrWhiteSpace(token))
            return Json(new { ok = false, message = "Sesión expirada." });

        var data = await _api.GenerarReporteDiarioAsync(fecha, token);
        if (data == null)
            return Json(new { ok = false, message = "No se pudo generar el reporte." });

        return Json(new { ok = true, data });
    }

    [HttpPost]
    public async Task<JsonResult> GenerarAuditoria(DateTime fechaDesde, DateTime fechaHasta)
    {
        var token = GetBearer();
        if (string.IsNullOrWhiteSpace(token))
            return Json(new { ok = false, message = "Sesión expirada." });

        var data = await _api.GenerarAuditoriaAsync(fechaDesde, fechaHasta, token);
        if (data == null)
            return Json(new { ok = false, message = "No se pudo generar la auditoría." });

        return Json(new { ok = true, data });
    }
}
