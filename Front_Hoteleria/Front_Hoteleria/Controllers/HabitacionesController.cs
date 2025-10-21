using Front_Hoteleria.Dto.Habitacion;
using Front_Hoteleria.Services.Api;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

public class HabitacionesController : Controller
{
    private readonly IHotelApiClient _api;

    public HabitacionesController() : this(new HotelApiClient()) { }
    public HabitacionesController(IHotelApiClient api) { _api = api; }

    // Helper unificado para leer el bearer
    private string GetBearer()
    {
        return (Session["Token"] as string)
               ?? (Request.Cookies["access_token"] != null ? Request.Cookies["access_token"].Value : null);
    }

    [HttpGet]
    public ActionResult Index() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> TablaPartial(int? vigencia, string nombre, bool? vip, int? capacidadMin)
    {
        var token = GetBearer();

        var data = await _api.HabitacionesDisponiblesAsync(vigencia ?? 1, token);

        if (!string.IsNullOrWhiteSpace(nombre))
            data = data.Where(x => (x.NombreHabitacion ?? string.Empty)
                        .IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

        if (vip.HasValue)
            data = data.Where(x => x.VIP == vip.Value).ToList();

        if (capacidadMin.HasValue)
            data = data.Where(x => x.Capacidad >= capacidadMin.Value).ToList();

        return PartialView("_TablaHabitaciones", data);
    }

    // Dashboard (estilo Upsert: arma modelo y devuelve partial)
    [HttpGet]
    public async Task<ActionResult> Dashboard(DateTime? desde, DateTime? hasta)
    {
        var token = GetBearer();

        // Defaults de fecha (últimos 30 días) si vienen nulas
        var d = desde ?? DateTime.Today.AddDays(-30);
        var h = hasta ?? DateTime.Today;

        var dto = "";//await  _api.DashboardHabitacionAsync(d, h, token);
                 // ?? new HabitacionDashboardDto();

        return PartialView("_DashboardHabitacion", dto);
    }

    [HttpGet]
    public async Task<ActionResult> Upsert(int? id)
    {
        var token = GetBearer();

        var dto = new HabitacionDto { Capacidad = 1, IdEstado = 1 };

        if (id.HasValue)
        {
            var lista = await _api.HabitacionesDisponiblesAsync(1, token);
            var existente = lista.FirstOrDefault(x => x.IdHabitacion == id.Value);
            if (existente != null) dto = existente;
        }

        return PartialView("_UpsertHabitacion", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Upsert(HabitacionDto dto)
    {
        var token = GetBearer();

        bool ok = dto.IdHabitacion == 0
            ? await _api.CrearHabitacionAsync(dto, token)
            : await _api.ModificarHabitacionAsync(dto, token);

        if (!ok) return new HttpStatusCodeResult(400, "No se pudo guardar.");
        return new HttpStatusCodeResult(200);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Eliminar(int idHabitacion)
    {
        var token = GetBearer();

        var ok = await _api.EliminarHabitacionAsync(idHabitacion, token);
        if (!ok) return new HttpStatusCodeResult(400, "No se pudo eliminar.");
        return new HttpStatusCodeResult(200);
    }
}
