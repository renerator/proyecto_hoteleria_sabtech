using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using Front_Hoteleria.Dto.Contrato;
using Front_Hoteleria.Services.Contratos;
using Front_Hoteleria.Services.Empresa;
using Front_Hoteleria.Services.Campamentos;
using Front_Hoteleria.Services.Trabajadores; // <-- NUEVO servicio front de trabajadores

using EmpresaDto = Front_Hoteleria.Dto.Empresa.EmpresaDto;
using CampamentoDto = Front_Hoteleria.Dto.Campamentos.CampamentoDto;

// OJO: tu DTO real de trabajadores está en *Font*_Hoteleria:
using Font_Hoteleria.Dto.Trabajadores;
using TrabajadoresDto = Font_Hoteleria.Dto.Trabajadores.TrabajadoresDto;

namespace Front_Hoteleria.Controllers
{
    public class ContratosController : Controller
    {
        private readonly IContratosService _api;
        private readonly IEmpresaService _empService;
        private readonly ICampamentosService _campService;
        private readonly ITrabajadoresService _trabService; // <-- NUEVO

        // ctor por defecto
        public ContratosController()
            : this(new ContratosService(), new EmpresaService(), new CampamentosService(), new TrabajadoresService()) { }

        // ctor con DI
        public ContratosController(
            IContratosService api,
            IEmpresaService empService,
            ICampamentosService campService,
            ITrabajadoresService trabService) // <-- NUEVO
        {
            _api = api;
            _empService = empService;
            _campService = campService;
            _trabService = trabService;      // <-- NUEVO
        }

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
                Trace.TraceError("[ContratosController.GetBearer] " + ex);
                return null;
            }
        }

        // ================== INDEX ==================
        [HttpGet]
        public ActionResult Index()
        {
            return View("~/Views/Contratos/Index.cshtml");
        }

        // ================== DASHBOARD (KPI) ==================
        // KPI: ahora suma trabajadores activos reales
        [HttpGet]
        public async Task<ActionResult> Resumen()
        {
            var token = GetBearer();

            var dto = await _api.ResumenAsync(token) ?? new ContratoKPIDto
            {
                ContratosActivos = 0,
                EmpresasRegistradas = 0,
                TrabajadoresActivos = 0,
                VencenPronto = 0
            };

            try
            {
                var contratos = await _api.ListarAsync(null, token) ?? new List<ContratoDto>();
                var empresaIds = contratos.Where(c => c.IdEmpresa > 0).Select(c => c.IdEmpresa.Value).Distinct().ToList();

                var tareas = empresaIds.ToDictionary(id => id, id => _trabService.ListarAsync(id, token));
                await Task.WhenAll(tareas.Values);

                var totalActivos = 0;
                foreach (var kv in tareas)
                {
                    var lista = kv.Value.Result;
                    totalActivos += lista.Count(t => t.Estado);
                }
                dto.TrabajadoresActivos = totalActivos;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("[ContratosController.Resumen] No se pudieron calcular trabajadores activos: " + ex);
            }

            return PartialView("~/Views/Contratos/_DashboardContrato.cshtml", dto);
        }

        // ================== TABLA ==================
        [HttpGet]
        [ActionName("Tabla")]
        public async Task<ActionResult> Tabla(string criterio)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada");

                // 1) Contratos
                var lista = await _api.ListarAsync(criterio, token)
                           ?? new List<ContratoDto>();

                // 2) Catálogos (empresas, campamentos) para enriquecer nombres
                var tEmpresas = _empService.ListarComboAsync(true, null, token);
                var tCampamentos = _campService.ListarComboAsync(null, null, token);
                await Task.WhenAll(tEmpresas, tCampamentos);

                var empresas = tEmpresas.Result ?? new List<EmpresaDto>();
                var campamentos = tCampamentos.Result ?? new List<Front_Hoteleria.Dto.Campamentos.CampamentoDto>();

                var dicEmp = empresas.GroupBy(e => e.IdEmpresa).ToDictionary(g => g.Key, g => g.First());
                var dicCamp = campamentos.GroupBy(c => c.IdCampamento).ToDictionary(g => g.Key, g => g.First());

                foreach (var c in lista)
                {
                    if (c == null) continue;

                    if ((c.IdEmpresa ?? 0) > 0 && dicEmp.TryGetValue(c.IdEmpresa.Value, out var emp))
                    {
                        c.Empresa = string.IsNullOrWhiteSpace(emp.Nombre) ? c.Empresa : emp.Nombre;
                        c.RutEmpresa = string.IsNullOrWhiteSpace(emp.Rut) ? c.RutEmpresa : emp.Rut;
                    }

                    if ((c.IdCampamento ?? 0) > 0 && dicCamp.TryGetValue(c.IdCampamento.Value, out var camp))
                    {
                        c.Campamento = string.IsNullOrWhiteSpace(camp.Nombre) ? c.Campamento : camp.Nombre;
                    }
                }

                // 3) Filtrado por criterio (texto)
                if (!string.IsNullOrWhiteSpace(criterio))
                {
                    var f = criterio.ToLower().Trim();
                    lista = lista.Where(c =>
                            (!string.IsNullOrWhiteSpace(c.Empresa) && c.Empresa.ToLower().Contains(f)) ||
                            (!string.IsNullOrWhiteSpace(c.NumeroContrato) && c.NumeroContrato.ToLower().Contains(f)) ||
                            (!string.IsNullOrWhiteSpace(c.RutEmpresa) && c.RutEmpresa.ToLower().Contains(f)) ||
                            (!string.IsNullOrWhiteSpace(c.Campamento) && c.Campamento.ToLower().Contains(f))
                        )
                        .ToList();
                }

                // 4) Traer TRABAJADORES por EmpresaContratista (IdEmpresa del contrato)
                var empresaIds = lista
                    .Where(c => c.IdEmpresa.HasValue && c.IdEmpresa.Value > 0)
                    .Select(c => c.IdEmpresa.Value)
                    .Distinct()
                    .ToList();

                var tareas = new Dictionary<int, Task<List<TrabajadoresDto>>>();
                foreach (var idEmp in empresaIds)
                {
                    tareas[idEmp] = _trabService.ListarAsync(idEmp, token); // /api/Trabajador/ListarTrabajadores?IdEmpresa=...
                }
                await Task.WhenAll(tareas.Values);

                // Diccionario final: EmpresaId -> Lista de trabajadores de esa empresa
                var trabPorEmpresa = new Dictionary<int, List<TrabajadoresDto>>();
                foreach (var kv in tareas)
                {
                    // Asegurar tipo correcto en ambos lados del '??'
                    var listaTrab = kv.Value.Result ?? new List<TrabajadoresDto>();
                    trabPorEmpresa[kv.Key] = listaTrab;
                }

                // 5) Enviar a la vista
                ViewBag.TrabajadoresEmpresa = trabPorEmpresa;

                return PartialView("~/Views/Contratos/_TablaContrato.cshtml", lista);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ContratosController.Tabla] " + ex);
                return new HttpStatusCodeResult(500, "Error al cargar contratos");
            }
        }



        // ================== NUEVO TRABAJADOR (modal) ==================
        [HttpGet]
        public async Task<ActionResult> NuevoTrabajador(int? idContrato)
        {
            var dto = new ContratoTrabajadorUpsertDto { IdContrato = idContrato };

            var token = GetBearer();

            // Si llega idContrato, intentamos preseleccionar su empresa
            int? empresaSeleccionada = null;
            if (idContrato.HasValue && idContrato.Value > 0)
            {
                try
                {
                    var con = await _api.ObtenerPorIdAsync(idContrato.Value, token);
                    empresaSeleccionada = con?.IdEmpresa;
                }
                catch { /* opcional log */ }
            }

            await CargarEmpresasEnViewBag(token, empresaSeleccionada);
            return PartialView("~/Views/Contratos/_UpsertTrabajador.cshtml", dto);
        }

        private static int ParseNivelAccesoForm(string nivelForm)
        {
            if (string.IsNullOrWhiteSpace(nivelForm)) return 1;
            if (int.TryParse(nivelForm, out var n))
            {
                if (n < 1) n = 1;
                if (n > 4) n = 4;
                return n;
            }
            switch (nivelForm.Trim().ToLowerInvariant())
            {
                case "admin": return 4;
                case "manager": return 3;
                case "worker": return 2;
                case "guest":
                default: return 1;
            }
        }

        [HttpPost]
        public async Task<ActionResult> GuardarTrabajador(ContratoTrabajadorUpsertDto dto)
        {
            if (dto == null)
                return Json(new { ok = false, msg = "Datos vacíos" });

            try
            {
                var token = GetBearer();

                var trabajador = new TrabajadoresDto
                {
                    IdUsuario = 0,
                    IdEmpresaContratista = dto.IdEmpresa ?? 0,
                    RutTrabajador = dto.Rut,
                    NombresTrabajador = dto.Nombre,
                    PaternoTrabajador = dto.Apellido,
                    MaternoTrabajador = null,
                    EmailTrabajador = dto.Email,
                    CargoTrabajador = dto.Cargo,
                    VIP = false,
                    EsAdmin = string.Equals(dto.NivelAcceso, "admin", StringComparison.OrdinalIgnoreCase)
                                          || dto.NivelAcceso == "4",
                    Estado = true,
                    Telefono = dto.Telefono,
                    NivelAcceso = ParseNivelAccesoForm(dto.NivelAcceso),
                    Observaciones = dto.Observaciones
                };

                var ok = await _trabService.CrearAsync(trabajador, token);
                if (!ok) return Json(new { ok = false, msg = "No se pudo crear el trabajador en la API." });

                return Json(new { ok = true, msg = "Trabajador creado exitosamente." });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ContratosController.GuardarTrabajador] " + ex);
                return Json(new { ok = false, msg = "Error inesperado al guardar trabajador" });
            }
        }

        // ================== NUEVA EMPRESA (modal simple) ==================
        [HttpGet]
        public ActionResult NuevaEmpresa()
        {
            var dto = new EmpresaContratoDto();
            return PartialView("~/Views/Contratos/_UpsertEmpresa.cshtml", dto);
        }

        [HttpPost]
        public async Task<ActionResult> GuardarEmpresa(EmpresaContratoDto dto)
        {
            if (dto == null)
                return Json(new { ok = false, msg = "Datos vacíos" });

            // Aquí puedes mapear a tu DTO de creación si lo tienes ya implementado.
            // Dejamos stub para mantener foco en trabajadores.
            try
            {
                // TODO: llamada real a API de empresa
                var ok = true;
                if (!ok) return Json(new { ok = false, msg = "No se pudo guardar en la API" });
                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ContratosController.GuardarEmpresa] " + ex);
                return Json(new { ok = false, msg = "Error inesperado al guardar empresa" });
            }
        }

        // ================== UPSERT (modal) ==================
        [HttpGet]
        public async Task<ActionResult> Upsert(int? id, bool? soloLectura)
        {
            var token = GetBearer();
            ContratoDto dto = null;

            if (id.HasValue && id.Value > 0)
            {
                dto = await _api.ObtenerPorIdAsync(id.Value, token);
            }

            if (dto == null)
            {
                dto = new ContratoDto
                {
                    IdContrato = id ?? 0,
                    Estado = true,
                    FechaInicio = DateTime.Today,
                    FechaFin = DateTime.Today.AddMonths(6)
                };
            }

            await CargarEmpresasEnViewBag(token, dto.IdEmpresa);
            await CargarCampamentosEnViewBag(token, dto.IdCampamento);

            ViewBag.SoloLectura = soloLectura == true;
            return PartialView("~/Views/Contratos/_UpsertContrato.cshtml", dto);
        }

        // ================== GUARDAR CONTRATO ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Guardar(ContratoDto dto)
        {
            if (dto == null)
                return Json(new { ok = false, msg = "Datos vacíos" });

            var token = GetBearer();

            try
            {
                if (dto.IdContrato == 0 && !dto.Estado) dto.Estado = true;

                var ok = dto.IdContrato > 0
                    ? await _api.ActualizarAsync(dto, token)
                    : await _api.CrearAsync(dto, token);

                if (!ok)
                    return Json(new { ok = false, msg = "No se pudo guardar en la API" });

                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ContratosController.Guardar] " + ex);
                return Json(new { ok = false, msg = "Error inesperado al guardar" });
            }
        }

        // ================== ELIMINAR ==================
        [HttpPost]
        public async Task<ActionResult> Eliminar(int id)
        {
            var token = GetBearer();
            try
            {
                var ok = await _api.EliminarAsync(id, token);
                return Json(new { ok });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ContratosController.Eliminar] " + ex);
                return Json(new { ok = false, msg = "Error al eliminar" });
            }
        }

        // ================== HELPERS: combos ==================
        private async Task CargarEmpresasEnViewBag(string token, int? seleccionada)
        {
            try
            {
                var empresas = await _empService.ListarComboAsync(true, null, token)
                               ?? new List<EmpresaDto>();

                var items = empresas
                    .OrderBy(e => e.Nombre)
                    .Select(e => new SelectListItem
                    {
                        Value = e.IdEmpresa.ToString(),
                        Text = string.IsNullOrWhiteSpace(e.Rut) ? e.Nombre : $"{e.Rut}-{e.Nombre} ",
                        Selected = seleccionada.HasValue && e.IdEmpresa == seleccionada.Value
                    })
                    .ToList();

                items.Insert(0, new SelectListItem { Value = "", Text = "Seleccionar empresa..." });
                ViewBag.Empresas = items;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ContratosController.CargarEmpresasEnViewBag] " + ex);
                ViewBag.Empresas = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Sin empresas disponibles" }
                };
            }
        }

        private async Task CargarCampamentosEnViewBag(string token, int? seleccionada)
        {
            try
            {
                var data = await _campService.ListarComboAsync(null, null, token)
                           ?? new List<CampamentoDto>();

                var items = data
                    .OrderBy(x => x.Nombre)
                    .Select(x => new SelectListItem
                    {
                        Value = x.IdCampamento.ToString(),
                        Text = string.IsNullOrWhiteSpace(x.Codigo) ? (x.Nombre ?? "") : $"{x.Nombre} ({x.Codigo})",
                        Selected = seleccionada.HasValue && x.IdCampamento == seleccionada.Value
                    })
                    .ToList();

                items.Insert(0, new SelectListItem { Value = "", Text = "Seleccionar campamento..." });
                ViewBag.Campamentos = items;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ContratosController.CargarCampamentosEnViewBag] " + ex);
                ViewBag.Campamentos = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Sin campamentos disponibles" }
                };
            }
        }
    }
}
