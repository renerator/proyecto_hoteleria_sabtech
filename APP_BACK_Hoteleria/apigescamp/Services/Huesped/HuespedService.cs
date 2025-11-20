using AutoMapper;
using DemoBackend.Dto.Huesped;
using DemoBackend.Models.Huesped;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Servicio;      // IHuespedService
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DemoBackend.Services.Huesped
{
    public class HuespedService : IHuespedService
    {
        private readonly IGenericRepositoryEntity<HuespedReclamoModels> _repoReclamos;
        private readonly IGenericRepositoryEntity<ReservaHuespedModels> _repoReserva;
        private readonly IGenericRepositoryEntity<EncuestaSatisfaccionModels> _repoEncuesta;
        private readonly IGenericRepositoryEntity<ServicioHuespedModels> _repoServicioHuesped;
        private readonly IMapper _mapper;

        public HuespedService(
            IGenericRepositoryEntity<HuespedReclamoModels> repoReclamos,
            IGenericRepositoryEntity<ReservaHuespedModels> repoReserva,
             IGenericRepositoryEntity<EncuestaSatisfaccionModels> repoEncuesta,
             IGenericRepositoryEntity<ServicioHuespedModels> repoServicioHuesped,
            IMapper mapper)
        {
            _repoReclamos = repoReclamos;
            _repoReserva = repoReserva;
            _repoEncuesta = repoEncuesta;
            _repoServicioHuesped = repoServicioHuesped;
            _mapper = mapper;
        }

        /// <summary>
        /// Crea un nuevo reclamo / sugerencia del huésped.
        /// </summary>
        public async Task<bool> CrearReclamoHuespedAsync(ReclamoSolicitudDto dto, string bearer)
        {
            // SP de inserción (ajusta el nombre si tu SP se llama distinto)
            const string sql = "HOT_CRE_HuespedReclamo " +
                               "@idTipoSolicitudHuesped,@TipoSolicitud," +
                               "@idCategoriaHuesped,@Categoria," +
                               "@Asunto,@Descripcion,@Email," +
                               "@idPrioridad,@Prioridad," +
                               "@Fecha,@idEstado,@Estado," +
                               "@Respuesta,@FechaRespuesta,@idUsuarioActualizacion";

            // Si no viene fecha, usamos ahora
            var fecha = dto.Fecha == default ? DateTime.Now : dto.Fecha;

            var parametros = new[]
            {
                new SqlParameter("@idTipoSolicitudHuesped", dto.IdTipoSolicitudHuesped),
                new SqlParameter("@TipoSolicitud", (object?)dto.TipoSolicitud ?? DBNull.Value),

                new SqlParameter("@idCategoriaHuesped", dto.IdCategoriaHuesped),
                new SqlParameter("@Categoria", (object?)dto.Categoria ?? DBNull.Value),

                new SqlParameter("@Asunto", (object?)dto.Asunto ?? DBNull.Value),
                new SqlParameter("@Descripcion", (object?)dto.Descripcion ?? DBNull.Value),
                new SqlParameter("@Email", (object?)dto.Email ?? DBNull.Value),

                new SqlParameter("@idPrioridad", dto.IdPrioridad),
                new SqlParameter("@Prioridad", (object?)dto.Prioridad ?? DBNull.Value),

                new SqlParameter("@Fecha", fecha),

                new SqlParameter("@idEstado", dto.IdEstado),
                new SqlParameter("@Estado", (object?)dto.Estado ?? DBNull.Value),

                new SqlParameter("@Respuesta", (object?)dto.Respuesta ?? DBNull.Value),
                new SqlParameter("@FechaRespuesta",
                    (object?)dto.FechaRespuesta ?? DBNull.Value),

                new SqlParameter("@idUsuarioActualizacion", dto.IdUsuarioActualizacion),
            };

            try
            {
                _repoReclamos.InsertProcedure(sql, parametros);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CrearReclamoHuespedAsync] {ex}");
                return await Task.FromResult(false);
            }
        }

        /// <summary>
        /// Lista los reclamos / sugerencias del huésped.
        /// </summary>
        public async Task<List<ReclamoSolicitudDto>> ListarReclamosHuespedAsync(string bearer)
        {
            // SP de listado (ajusta el nombre si tu SP se llama distinto)
            const string sql = "HOT_LIST_HuespedReclamo";

            try
            {
                var listaModels = _repoReclamos.GetStoreProcedure(sql);
                var listaDto = _mapper.Map<List<ReclamoSolicitudDto>>(listaModels);
                return await Task.FromResult(listaDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ListarReclamosHuespedAsync] {ex}");
                return await Task.FromResult(new List<ReclamoSolicitudDto>());
            }
        }

        public ReclamoSolicitudDto ObtenerReclamoHuespedPorId(int idReclamoHuesped)
        {
            const string sql = "HOT_GET_HuespedReclamoById @idReclamoHuesped";
            var parametros = new[]
            {
                new SqlParameter("@idReclamoHuesped", idReclamoHuesped)
            };

            var lista = _repoReclamos.GetStoreProcedure(sql, parametros);
            var entidad = lista.FirstOrDefault();
            return _mapper.Map<ReclamoSolicitudDto>(entidad);
        }


        /// <summary>
        /// Lista reservas del huésped (filtro por código, estado, fechas).
        /// SP: HOT_HUESPED_RESERVA_LISTAR @Codigo,@IdEstado,@FechaDesde,@FechaHasta
        /// </summary>
        public List<ReservaHuespedDto> Buscar(ReservaHuespedDto filtro)
        {
            const string sql = "HOT_HUESPED_RESERVA_LISTAR @Codigo,@IdEstado,@FechaDesde,@FechaHasta";

            var p = new SqlParameter[]
            {
        new SqlParameter("@Codigo",    (object?)filtro.FiltroCodigo   ?? DBNull.Value),
        new SqlParameter("@IdEstado",  (object?)filtro.FiltroIdEstado ?? DBNull.Value),
        new SqlParameter("@FechaDesde",(object?)filtro.FiltroDesde    ?? DBNull.Value),
        new SqlParameter("@FechaHasta",(object?)filtro.FiltroHasta    ?? DBNull.Value)
            };

            try
            {
                var lista = _repoReserva.GetStoreProcedure(sql, p);

                if (lista == null)
                    return new List<ReservaHuespedDto>();

                return lista
                    .Select(r => new ReservaHuespedDto
                    {
                        IdReserva = r.IdReserva,
                        CodigoReserva = r.CodigoReserva,
                        IdTrabajador = r.IdTrabajador,
                        IdTipoReserva = r.IdTipoReserva,
                        Nombre = r.Nombre,
                        Apellido = r.Apellido,
                        Email = r.Email,
                        Telefono = r.Telefono,
                        FechaSolicitud = r.FechaSolicitud,
                        FechaDesde = r.FechaDesde,
                        FechaHasta = r.FechaHasta,
                        DiasEstadia = r.DiasEstadia,
                        IdEstadoReserva = r.IdEstadoReserva,
                        Estado = r.Estado,
                        Comentarios = r.Comentarios
                    })
                    .OrderByDescending(x => x.FechaSolicitud)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Logueas el error y devuelves lista vacía
                Trace.TraceError("[ReservaHuespedRepository.Buscar] " + ex);
                return new List<ReservaHuespedDto>();
            }
        }


        /// <summary>
        /// SP: HOT_HUESPED_RESERVA_DETALLE @IdReserva
        /// </summary>

public ReservaHuespedDto ObtenerPorId(int idReserva)
    {
        if (idReserva <= 0)
            return null;

        try
        {
            const string sql = "HOT_HUESPED_RESERVA_DETALLE @IdReserva";
            var p = new SqlParameter[]
            {
            new SqlParameter("@IdReserva", idReserva)
            };

            var lista = _repoReserva.GetStoreProcedure(sql, p);

            if (lista == null)
                return null;

            var e = lista.FirstOrDefault();
            if (e == null)
                return null;

            return new ReservaHuespedDto
            {
                IdReserva = e.IdReserva,
                CodigoReserva = e.CodigoReserva,
                IdTrabajador = e.IdTrabajador,
                IdTipoReserva = e.IdTipoReserva,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                Email = e.Email,
                Telefono = e.Telefono,
                FechaSolicitud = e.FechaSolicitud,
                FechaDesde = e.FechaDesde,
                FechaHasta = e.FechaHasta,
                DiasEstadia = e.DiasEstadia,
                IdEstadoReserva = e.IdEstadoReserva,
                Estado = e.Estado,
                Comentarios = e.Comentarios
            };
        }
        catch (Exception ex)
        {
            Trace.TraceError("[ReservaHuespedRepository.ObtenerPorId] " + ex);
            return null; // o lanza la excepción si quieres que suba a la capa superior
        }
    }

        public bool RegistrarEncuesta(EncuestaSatisfaccionDto dto)
        {
            const string sql = "HOT_ENCUESTA_SATISFACCION_INS " +
                               "@IdReserva, @TipoEncuesta, " +
                               "@CalificacionGeneral, @AtencionPersonal, " +
                               "@LimpiezaHabitacion, @FacilidadesHotel, " +
                               "@RelacionCalidadPrecio, @Comentarios, @Recomendaria, @IdUsuarioCreacion";

            var p = new SqlParameter[]
            {
                new SqlParameter("@IdReserva", (object?)dto.IdReserva ?? DBNull.Value),
                new SqlParameter("@TipoEncuesta", dto.TipoEncuesta),

                new SqlParameter("@CalificacionGeneral",  (object?)dto.CalificacionGeneral   ?? DBNull.Value),
                new SqlParameter("@AtencionPersonal",     (object?)dto.AtencionPersonal      ?? DBNull.Value),
                new SqlParameter("@LimpiezaHabitacion",   (object?)dto.LimpiezaHabitacion    ?? DBNull.Value),
                new SqlParameter("@FacilidadesHotel",     (object?)dto.FacilidadesHotel      ?? DBNull.Value),
                new SqlParameter("@RelacionCalidadPrecio",(object?)dto.RelacionCalidadPrecio ?? DBNull.Value),

                new SqlParameter("@Comentarios",          (object?)dto.Comentarios ?? DBNull.Value),
                new SqlParameter("@Recomendaria",         (object?)dto.Recomendaria ?? DBNull.Value),

                // si tienes el usuario actual en contexto, pásalo aquí. De momento 1 fijo.
                new SqlParameter("@IdUsuarioCreacion", 1)
            };

            try
            {
                _repoEncuesta.InsertProcedure(sql, p);
                return true;

               
            }
            catch (Exception ex)
            {
                Trace.TraceError("[HuespedService.RegistrarEncuesta] " + ex);
                return false;
            }
        }

        /// <summary>
        /// SP: HOT_HUESPED_RESERVA_CREAR
        /// @IdTrabajador,@IdTipoReserva,@Nombre,@Apellido,@Email,@Telefono,
        /// @FechaDesde,@FechaHasta,@Comentarios
        /// </summary>
        public bool Crear(ReservaHuespedDto dto)
        {
            const string sql =
                "HOT_HUESPED_RESERVA_CREAR " +
                "@IdTrabajador,@IdTurno,@Email," +
                "@FechaDesde,@FechaHasta,@Comentarios";

            var p = new SqlParameter[]
            {
        new SqlParameter("@IdTrabajador", dto.IdTrabajador),
        new SqlParameter("@IdTurno", dto.IdTurno),
        new SqlParameter("@Email", (object?)dto.Email ?? DBNull.Value),
        new SqlParameter("@FechaDesde", dto.FechaDesde),
        new SqlParameter("@FechaHasta", dto.FechaHasta),
        new SqlParameter("@Comentarios", (object?)dto.Comentarios ?? DBNull.Value)
            };

            try
            {
                _repoReserva.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReservaHuespedService.Crear] {ex}");
                return false;
            }
        }


        /// <summary>
        /// SP: HOT_HUESPED_RESERVA_ACTUALIZAR
        /// </summary>
        public bool Actualizar(ReservaHuespedDto dto)
        {
            const string sql =
                "HOT_HUESPED_RESERVA_ACTUALIZAR " +
                "@IdReserva,@Nombre,@Apellido,@Email,@Telefono,@FechaDesde,@FechaHasta,@Comentarios";

            var p = new SqlParameter[]
            {
                new SqlParameter("@IdReserva", dto.IdReserva),
                new SqlParameter("@Nombre", (object?)dto.Nombre ?? DBNull.Value),
                new SqlParameter("@Apellido", (object?)dto.Apellido ?? DBNull.Value),
                new SqlParameter("@Email", (object?)dto.Email ?? DBNull.Value),
                new SqlParameter("@Telefono", (object?)dto.Telefono ?? DBNull.Value),
                new SqlParameter("@FechaDesde", dto.FechaDesde),
                new SqlParameter("@FechaHasta", dto.FechaHasta),
                new SqlParameter("@Comentarios", (object?)dto.Comentarios ?? DBNull.Value)
            };

            try
            {
                _repoReserva.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReservaHuespedService.Actualizar] {ex}");
                return false;
            }
        }

        /// <summary>
        /// SP: HOT_HUESPED_RESERVA_ELIMINAR @IdReserva
        /// </summary>
        public bool Eliminar(int idReserva)
        {
            const string sql = "HOT_HUESPED_RESERVA_ELIMINAR @IdReserva";

            var p = new SqlParameter[]
            {
                new SqlParameter("@IdReserva", idReserva)
            };

            try
            {
                _repoReserva.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReservaHuespedService.Eliminar] {ex}");
                return false;
            }
        }
        // ================== SERVICIO HUESPED ==================

        // ================== SERVICIO HUESPED ==================

        /// <summary>
        /// Lista solicitudes de servicio de huésped con filtros:
        /// SP: HOT_HUESPED_SERVICIO_LISTAR @IdEstado,@FechaDesde,@FechaHasta,@Texto,@NombreServicio
        /// </summary>
        public List<ServicioHuespedDto> BuscarServiciosHuesped(ServicioHuespedDto filtro)
        {
            const string sql = "HOT_HUESPED_SERVICIO_LISTAR " +
                               "@IdEstado,@FechaDesde,@FechaHasta,@Texto,@NombreServicio";

            var p = new SqlParameter[]
            {
        new SqlParameter("@IdEstado",        (object?)filtro.FiltroIdEstado       ?? DBNull.Value),
        new SqlParameter("@FechaDesde",      (object?)filtro.FiltroDesde          ?? DBNull.Value),
        new SqlParameter("@FechaHasta",      (object?)filtro.FiltroHasta          ?? DBNull.Value),
        new SqlParameter("@Texto",           (object?)filtro.FiltroTexto          ?? DBNull.Value),
        new SqlParameter("@NombreServicio",  (object?)filtro.FiltroNombreServicio ?? DBNull.Value)
            };

            try
            {
                var lista = _repoServicioHuesped.GetStoreProcedure(sql, p);

                if (lista == null)
                    return new List<ServicioHuespedDto>();

                return lista
                    .Select(e => new ServicioHuespedDto
                    {
                        IdSolicitudServicio = e.IdSolicitudServicio,

                        IdTipoServicio = e.IdTipoServicio,
                        TipoServicio = e.TipoServicio,

                        IdPrioridad = e.IdPrioridad,
                        Prioridad = e.Prioridad,

                        Descripcion = e.Descripcion,
                        FechaPreferida = e.FechaPreferida,

                        IdMetodoContacto = e.IdMetodoContacto,
                        MetodoContacto = e.MetodoContacto,

                        ComentariosAdicionales = e.ComentariosAdicionales,

                        IdEstado = e.IdEstado,
                        Estado = e.Estado,

                        FechaSolicitud = e.FechaSolicitud,

                        Nombre = e.Nombre,
                        Apellido = e.Apellido,
                        Email = e.Email
                    })
                    .OrderByDescending(x => x.FechaSolicitud)
                    .ToList();
            }
            catch (Exception ex)
            {
                Trace.TraceError("[HuespedService.BuscarServiciosHuesped] " + ex);
                return new List<ServicioHuespedDto>();
            }
        }

        /// <summary>
        /// Detalle de una solicitud de servicio de huésped.
        /// SP: HOT_HUESPED_SERVICIO_DETALLE @IdSolicitudServicio
        /// </summary>
        public ServicioHuespedDto ObtenerServicioHuespedPorId(int idSolicitudServicio)
        {
            if (idSolicitudServicio <= 0)
                return null;

            const string sql = "HOT_HUESPED_SERVICIO_DETALLE @IdSolicitudServicio";

            var p = new SqlParameter[]
            {
        new SqlParameter("@IdSolicitudServicio", idSolicitudServicio)
            };

            try
            {
                var lista = _repoServicioHuesped.GetStoreProcedure(sql, p);
                var e = lista?.FirstOrDefault();
                if (e == null) return null;

                return new ServicioHuespedDto
                {
                    IdSolicitudServicio = e.IdSolicitudServicio,

                    IdTipoServicio = e.IdTipoServicio,
                    TipoServicio = e.TipoServicio,

                    IdPrioridad = e.IdPrioridad,
                    Prioridad = e.Prioridad,

                    Descripcion = e.Descripcion,
                    FechaPreferida = e.FechaPreferida,

                    IdMetodoContacto = e.IdMetodoContacto,
                    MetodoContacto = e.MetodoContacto,

                    ComentariosAdicionales = e.ComentariosAdicionales,

                    IdEstado = e.IdEstado,
                    Estado = e.Estado,

                    FechaSolicitud = e.FechaSolicitud,

                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    Email = e.Email
                };
            }
            catch (Exception ex)
            {
                Trace.TraceError("[HuespedService.ObtenerServicioHuespedPorId] " + ex);
                return null;
            }
        }

        /// <summary>
        /// Crea una nueva solicitud de servicio de huésped.
        /// SP: HOT_HUESPED_SERVICIO_CREAR
        /// Retorna un Id &gt; 0 si se creó correctamente.
        /// </summary>
        public int CrearServicioHuesped(ServicioHuespedDto dto)
        {
            const string sql =
                "HOT_HUESPED_SERVICIO_CREAR " +
                "@IdTipoServicio,@TipoServicio," +
                "@IdPrioridad,@Prioridad," +
                "@Descripcion,@FechaPreferida," +
                "@IdMetodoContacto,@MetodoContacto," +
                "@ComentariosAdicionales," +
                "@IdEstado,@Estado," +
                "@FechaSolicitud,@IdUsuarioActualizacion";

            var fechaSolicitud = dto.FechaSolicitud == default
                ? DateTime.Now
                : dto.FechaSolicitud;

            var p = new SqlParameter[]
            {
        new SqlParameter("@IdTipoServicio",        (object?)dto.IdTipoServicio        ?? DBNull.Value),
        new SqlParameter("@TipoServicio",          (object?)dto.TipoServicio          ?? DBNull.Value),

        new SqlParameter("@IdPrioridad",           (object?)dto.IdPrioridad           ?? DBNull.Value),
        new SqlParameter("@Prioridad",             (object?)dto.Prioridad             ?? DBNull.Value),

        new SqlParameter("@Descripcion",           (object?)dto.Descripcion           ?? DBNull.Value),
        new SqlParameter("@FechaPreferida",        (object?)dto.FechaPreferida        ?? DBNull.Value),

        new SqlParameter("@IdMetodoContacto",      (object?)dto.IdMetodoContacto      ?? DBNull.Value),
        new SqlParameter("@MetodoContacto",        (object?)dto.MetodoContacto        ?? DBNull.Value),

        new SqlParameter("@ComentariosAdicionales",(object?)dto.ComentariosAdicionales?? DBNull.Value),

        new SqlParameter("@IdEstado",              (object?)dto.IdEstado              ?? DBNull.Value),
        new SqlParameter("@Estado",                (object?)dto.Estado                ?? DBNull.Value),

        new SqlParameter("@FechaSolicitud",        fechaSolicitud),

        // de momento fijo = 1 (ajusta cuando tengas usuario en contexto)
        new SqlParameter("@IdUsuarioActualizacion", 1)
            };

            try
            {
                // Si tu SP devuelve el Id por OUTPUT o SELECT, aquí deberías usar un método que lo lea.
                // Como tu repositorio actual sólo hace InsertProcedure (void), devolvemos 1 para indicar OK.
                _repoServicioHuesped.InsertProcedure(sql, p);
                return 1;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[HuespedService.CrearServicioHuesped] " + ex);
                return 0;
            }
        }

        /// <summary>
        /// Actualiza una solicitud de servicio de huésped.
        /// SP: HOT_HUESPED_SERVICIO_ACTUALIZAR
        /// </summary>
        public bool ActualizarServicioHuesped(ServicioHuespedDto dto)
        {
            if (dto.IdSolicitudServicio <= 0)
                return false;

            const string sql =
                "HOT_HUESPED_SERVICIO_ACTUALIZAR " +
                "@IdSolicitudServicio," +
                "@IdTipoServicio,@TipoServicio," +
                "@IdPrioridad,@Prioridad," +
                "@Descripcion,@FechaPreferida," +
                "@IdMetodoContacto,@MetodoContacto," +
                "@ComentariosAdicionales," +
                "@IdEstado,@Estado," +
                "@IdUsuarioActualizacion";

            var p = new SqlParameter[]
            {
        new SqlParameter("@IdSolicitudServicio",   dto.IdSolicitudServicio),

        new SqlParameter("@IdTipoServicio",        (object?)dto.IdTipoServicio        ?? DBNull.Value),
        new SqlParameter("@TipoServicio",          (object?)dto.TipoServicio          ?? DBNull.Value),

        new SqlParameter("@IdPrioridad",           (object?)dto.IdPrioridad           ?? DBNull.Value),
        new SqlParameter("@Prioridad",             (object?)dto.Prioridad             ?? DBNull.Value),

        new SqlParameter("@Descripcion",           (object?)dto.Descripcion           ?? DBNull.Value),
        new SqlParameter("@FechaPreferida",        (object?)dto.FechaPreferida        ?? DBNull.Value),

        new SqlParameter("@IdMetodoContacto",      (object?)dto.IdMetodoContacto      ?? DBNull.Value),
        new SqlParameter("@MetodoContacto",        (object?)dto.MetodoContacto        ?? DBNull.Value),

        new SqlParameter("@ComentariosAdicionales",(object?)dto.ComentariosAdicionales?? DBNull.Value),

        new SqlParameter("@IdEstado",              (object?)dto.IdEstado              ?? DBNull.Value),
        new SqlParameter("@Estado",                (object?)dto.Estado                ?? DBNull.Value),

        new SqlParameter("@IdUsuarioActualizacion", 1)
            };

            try
            {
                _repoServicioHuesped.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[HuespedService.ActualizarServicioHuesped] " + ex);
                return false;
            }
        }

        /// <summary>
        /// Elimina / da de baja una solicitud de servicio de huésped.
        /// SP: HOT_HUESPED_SERVICIO_ELIMINAR @IdSolicitudServicio
        /// </summary>
        public bool EliminarServicioHuesped(int idSolicitudServicio)
        {
            if (idSolicitudServicio <= 0)
                return false;

            const string sql = "HOT_HUESPED_SERVICIO_ELIMINAR @IdSolicitudServicio";

            var p = new SqlParameter[]
            {
        new SqlParameter("@IdSolicitudServicio", idSolicitudServicio)
            };

            try
            {
                _repoServicioHuesped.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("[HuespedService.EliminarServicioHuesped] " + ex);
                return false;
            }
        }



    }
}
