using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Front_Hoteleria.Dto.Habitacion;
using Front_Hoteleria.Services.Api;

public class HabitacionesController : Controller
{
    private readonly IHotelApiClient _api;

    
    public HabitacionesController() : this(new HotelApiClient()) { }

   
    public HabitacionesController(IHotelApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public ActionResult Index() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> TablaPartial(int? vigencia, string nombre, int? capacidadMin)
    {
        var token = Session["Token"] as string
                    ?? (Request.Cookies["access_token"] != null ? Request.Cookies["access_token"].Value : null);

        var data = await _api.HabitacionesDisponiblesAsync(vigencia ?? 1, token);

        if (!string.IsNullOrWhiteSpace(nombre))
            data = data.Where(x => ((x.Area?? string.Empty))
                        .IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

        if (capacidadMin.HasValue)
            data = data.Where(x => x.Capacidad >= capacidadMin.Value).ToList();

        return PartialView("_TablaHabitaciones", data);
    }

    [HttpGet]
    public async Task<ActionResult> Upsert(int? id)
    {
        var dto = new HabitacionDto { Capacidad = 1, IdEstado = 1 };
        if (id.HasValue)
        {
            var lista = await _api.HabitacionesDisponiblesAsync(1);
            var existente = lista.FirstOrDefault(x => x.IdHabitacion == id.Value);
            if (existente != null) dto = existente;
        }
        return PartialView("_UpsertHabitacion", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Upsert(HabitacionDto dto)
    {
        var token = Session["access_token"] as string
                    ?? (Request.Cookies["access_token"] != null ? Request.Cookies["access_token"].Value : null);

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
        var token = Session["access_token"] as string
                    ?? (Request.Cookies["access_token"] != null ? Request.Cookies["access_token"].Value : null);

        var ok = await _api.EliminarHabitacionAsync(idHabitacion, token);
        if (!ok) return new HttpStatusCodeResult(400, "No se pudo eliminar.");
        return new HttpStatusCodeResult(200);
    }
}
