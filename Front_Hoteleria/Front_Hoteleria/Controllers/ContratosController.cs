using Front_Hoteleria.Dto.Contrato;
using Front_Hoteleria.Services.Contratos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Front_Hoteleria.Controllers
{
    public class ContratosController : Controller
    {
        private readonly IContratosService _api;

        // ctor por defecto
        public ContratosController() : this(new ContratosService()) { }

        // ctor con inyección
        public ContratosController(IContratosService api)
        {
            _api = api;
        }

        // leer el bearer igual que en dotaciones
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

        // GET: /Contratos
        [HttpGet]
        public ActionResult Index()
        {
            // ~/Views/Contratos/Index.cshtml
            return View("~/Views/Contratos/Index.cshtml");
        }

        // PANEL AZUL (KPI)
        // GET: /Contratos/Resumen
        [HttpGet]
        public async Task<ActionResult> Resumen()
        {
            var token = GetBearer();

            // intenta ir a la api
            var dto = await _api.ResumenAsync(token);
            if (dto == null)
            {
                // datos en duro para ver diseño
                dto = new ContratoKPIDto
                {
                    ContratosActivos = 8,
                    EmpresasRegistradas = 12,
                    TrabajadoresActivos = 156,
                    VencenPronto = 3
                };
            }

            // ~/Views/Contratos/_ResumenContratos.cshtml
            return PartialView("~/Views/Contratos/_DashboardContrato.cshtml", dto);
        }

        // LISTA / tarjetas
        // GET: /Contratos/Tabla
        [HttpGet]
        [ActionName("Tabla")]
        public async Task<ActionResult> Tabla(string criterio)
        {
            try
            {
                var token = GetBearer();
                if (string.IsNullOrWhiteSpace(token))
                    return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized, "Sesión expirada");

                // 1) intentamos traer de la API
                var lista = await _api.ListarAsync(criterio, token);

                // 2) si la api no devolvió nada, armamos datos en duro para ver el diseño
                if (lista == null || !lista.Any())
                {
                    lista = new List<ContratoDto>
                    {
                        new ContratoDto
                        {
                            IdContrato = 1,
                            IdEmpresa = 1,
                            Empresa = "Constructora ABC Ltda.",
                            RutEmpresa = "12.345.678-9",
                            NumeroContrato = "CONT-2024-001",
                            FechaInicio = DateTime.Today.AddMonths(-2),
                            FechaFin = DateTime.Today.AddMonths(10),
                            Tipo = "indefinido",
                            Valor = 1500000,
                            IdCampamento = 1,
                            Campamento = "Campamento Norte",
                            MaximoTrabajadores = 50,
                            Descripcion = "Contrato principal de construcción",
                            Estado = "Activo",
                            Trabajadores = new List<ContratoTrabajadorDto>
                            {
                                new ContratoTrabajadorDto
                                {
                                    IdTrabajador = 1,
                                    Nombres = "Juan Pérez",
                                    Rut = "12.345.678-9",
                                    Cargo = "Supervisor",
                                    NivelAcceso = "manager"
                                },
                                new ContratoTrabajadorDto
                                {
                                    IdTrabajador = 2,
                                    Nombres = "Carlos Méndez",
                                    Rut = "11.222.333-4",
                                    Cargo = "Operador",
                                    NivelAcceso = "worker"
                                }
                            }
                        },
                        new ContratoDto
                        {
                            IdContrato = 2,
                            IdEmpresa = 2,
                            Empresa = "Servicios Mineros XYZ S.A.",
                            RutEmpresa = "98.765.432-1",
                            NumeroContrato = "CONT-2024-002",
                            FechaInicio = DateTime.Today.AddMonths(-1),
                            FechaFin = DateTime.Today.AddDays(25),  // para que salga "vence pronto"
                            Tipo = "proyecto",
                            Valor = 800000,
                            IdCampamento = 2,
                            Campamento = "Campamento Sur",
                            MaximoTrabajadores = 25,
                            Descripcion = "Contrato de servicios de mantenimiento",
                            Estado = "Activo",
                            Trabajadores = new List<ContratoTrabajadorDto>
                            {
                                new ContratoTrabajadorDto
                                {
                                    IdTrabajador = 3,
                                    Nombres = "María González",
                                    Rut = "13.456.789-0",
                                    Cargo = "Gerente",
                                    NivelAcceso = "admin"
                                }
                            }
                        }
                    };
                }

                // 3) si vino criterio, filtramos igual que en dotaciones
                if (!string.IsNullOrWhiteSpace(criterio))
                {
                    var f = criterio.ToLower().Trim();
                    lista = lista
                        .Where(c =>
                            (!string.IsNullOrWhiteSpace(c.Empresa) && c.Empresa.ToLower().Contains(f)) ||
                            (!string.IsNullOrWhiteSpace(c.NumeroContrato) && c.NumeroContrato.ToLower().Contains(f)) ||
                            (!string.IsNullOrWhiteSpace(c.RutEmpresa) && c.RutEmpresa.ToLower().Contains(f)) ||
                            (!string.IsNullOrWhiteSpace(c.Campamento) && c.Campamento.ToLower().Contains(f))
                        )
                        .ToList();
                }

                // ~/Views/Contratos/_TablaContratos.cshtml
                return PartialView("~/Views/Contratos/_TablaContrato.cshtml", lista);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ContratosController.Tabla] " + ex);
                return new HttpStatusCodeResult(500, "Error al cargar contratos");
            }
        }
        // GET: /Contratos/NuevoTrabajador
        [HttpGet]
        public ActionResult NuevoTrabajador(int? idContrato)
        {
            // puedes precargar el contrato si quieres, por ahora solo mando el id
            var dto = new ContratoTrabajadorUpsertDto
            {
                IdContrato = idContrato
            };

            return PartialView("~/Views/Contratos/_UpsertTrabajador.cshtml", dto);
        }

        // POST: /Contratos/GuardarTrabajador
        [HttpPost]
        public async Task<ActionResult> GuardarTrabajador(ContratoTrabajadorUpsertDto dto)
        {
            if (dto == null)
                return Json(new { ok = false, msg = "Datos vacíos" });

            var token = GetBearer();

            try
            {
                // acá iría la llamada real a la API, algo como:
                // var ok = await _api.AgregarTrabajadorAsync(dto, token);
                // demo:
                var ok = true;

                if (!ok)
                    return Json(new { ok = false, msg = "No se pudo guardar en la API" });

                return Json(new { ok = true });
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.TraceError("[ContratosController.GuardarTrabajador] " + ex);
                return Json(new { ok = false, msg = "Error inesperado al guardar trabajador" });
            }
        }

        // GET: /Contratos/NuevaEmpresa
        [HttpGet]
        public ActionResult NuevaEmpresa()
        {
            // solo para mostrar el modal vacío
            var dto = new EmpresaContratoDto();
            return PartialView("~/Views/Contratos/_UpsertEmpresa.cshtml", dto);
        }

        // POST: /Contratos/GuardarEmpresa
        [HttpPost]
        public async Task<ActionResult> GuardarEmpresa(EmpresaContratoDto dto)
        {
            if (dto == null)
                return Json(new { ok = false, msg = "Datos vacíos" });

            var token = GetBearer();

            try
            {
                // aquí iría la llamada real a la API, algo como:
                // var ok = await _api.CrearEmpresaAsync(dto, token);
                // por ahora demo:
                var ok = true;

                if (!ok)
                    return Json(new { ok = false, msg = "No se pudo guardar en la API" });

                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                Trace.TraceError("[ContratosController.GuardarEmpresa] " + ex);
                return Json(new { ok = false, msg = "Error inesperado al guardar empresa" });
            }
        }

        // MODAL (nuevo / editar / ver)
        // GET: /Contratos/Upsert
        [HttpGet]
        public async Task<ActionResult> Upsert(int? id, bool? soloLectura)
        {
            var token = GetBearer();
            ContratoDto dto = null;

            if (id.HasValue && id.Value > 0)
            {
                // intentar traer de la api
                dto = await _api.ObtenerPorIdAsync(id.Value, token);
            }

            if (dto == null)
            {
                dto = new ContratoDto
                {
                    IdContrato = id ?? 0,
                    Estado = "Activo",
                    FechaInicio = DateTime.Today,
                    FechaFin = DateTime.Today.AddMonths(6)
                };
            }

            // le pasas por querystring ?soloLectura=true y en la vista ya lo capturaste
            return PartialView("~/Views/Contratos/_UpsertContrato.cshtml", dto);
        }

        // guardar (demo)
        // POST: /Contratos/Guardar
        [HttpPost]
        public async Task<ActionResult> Guardar(ContratoDto dto)
        {
            if (dto == null)
                return Json(new { ok = false, msg = "Datos vacíos" });

            var token = GetBearer();

            try
            {
                bool ok;
                if (dto.IdContrato > 0)
                {
                    ok = await _api.ActualizarAsync(dto, token);
                }
                else
                {
                    ok = await _api.CrearAsync(dto, token);
                }

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

        // opcional: eliminar
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
    }
}
