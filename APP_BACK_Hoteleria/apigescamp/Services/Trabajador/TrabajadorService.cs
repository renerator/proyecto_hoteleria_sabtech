using AutoMapper;
using DemoBackend.Dto.Trabajador;
using DemoBackend.Models.Trabajador;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Trabajador;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoBackend.Services
{
    public class TrabajadorService : ITrabajadorService
    {
        private readonly IGenericRepositoryEntity<TrabajadorModels> _repoTrabajador;
        private readonly IMapper _mapper;
        private readonly ILogger<TrabajadorService> _logger;
        //cambio 1-12
        public TrabajadorService(
            IGenericRepositoryEntity<TrabajadorModels> repoTrabajador,
            IMapper mapper,
            ILogger<TrabajadorService> logger)
        {
            _repoTrabajador = repoTrabajador;
            _mapper = mapper;
            _logger = logger;
        }

        #region Trabajador (CRUD + verificaciones + listados)

        // ===== CREATE =====
        public bool CrearTrabajador(TrabajadorDto trabajador)
        {
            // SP actualizado para usar Rut + columnas nuevas
            string sql =
                "TRA_CRE_Trabajador " +
                "@idEmpresaContratista," +
                "@RutTrabajador," +
                "@NombresTrabajador," +
                "@PaternoTrabajador," +
                "@MaternoTrabajador," +
                "@EmailTrabajador," +
                "@CargoTrabajador," +
                "@VIP," +
                "@EsAdmin," +
                "@Estado," +
                "@NivelAcceso," +
                "@Observaciones";

            var p = new SqlParameter[]
            {
                new SqlParameter("@idEmpresaContratista", trabajador.IdEmpresaContratista),
                new SqlParameter("@RutTrabajador", (object?)trabajador.RutTrabajador ?? DBNull.Value),
                new SqlParameter("@NombresTrabajador", (object?)trabajador.NombresTrabajador ?? DBNull.Value),
                new SqlParameter("@PaternoTrabajador", (object?)trabajador.PaternoTrabajador ?? DBNull.Value),
                new SqlParameter("@MaternoTrabajador", (object?)trabajador.MaternoTrabajador ?? DBNull.Value),
                new SqlParameter("@EmailTrabajador", (object?)trabajador.EmailTrabajador ?? DBNull.Value),
                new SqlParameter("@CargoTrabajador", (object?)trabajador.CargoTrabajador ?? DBNull.Value),
                new SqlParameter("@VIP", trabajador.VIP),
                new SqlParameter("@EsAdmin", trabajador.EsAdmin),
                new SqlParameter("@Estado", trabajador.Estado),
                new SqlParameter("@NivelAcceso", trabajador.NivelAcceso),              // NUEVO
                new SqlParameter("@Observaciones", (object?)trabajador.Observaciones ?? DBNull.Value) // NUEVO
            };

            try
            {
                _repoTrabajador.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CrearTrabajador] Error");
                return false;
            }
        }

        // ===== UPDATE =====
        public bool ModificarTrabajador(TrabajadorDto trabajador)
        {
            string sql =
                "TRA_UPD_Trabajador " +
                "@idUsuario," +
                "@idEmpresaContratista," +
                "@RutTrabajador," +
                "@NombresTrabajador," +
                "@PaternoTrabajador," +
                "@MaternoTrabajador," +
                "@EmailTrabajador," +
                "@CargoTrabajador," +
                "@VIP," +
                "@EsAdmin," +
                "@Estado," +
                "@NivelAcceso," +
                "@Observaciones";

            var p = new SqlParameter[]
            {
                new SqlParameter("@idUsuario", trabajador.IdUsuario),
                new SqlParameter("@idEmpresaContratista", trabajador.IdEmpresaContratista),
                new SqlParameter("@RutTrabajador", (object?)trabajador.RutTrabajador ?? DBNull.Value),
                new SqlParameter("@NombresTrabajador", (object?)trabajador.NombresTrabajador ?? DBNull.Value),
                new SqlParameter("@PaternoTrabajador", (object?)trabajador.PaternoTrabajador ?? DBNull.Value),
                new SqlParameter("@MaternoTrabajador", (object?)trabajador.MaternoTrabajador ?? DBNull.Value),
                new SqlParameter("@EmailTrabajador", (object?)trabajador.EmailTrabajador ?? DBNull.Value),
                new SqlParameter("@CargoTrabajador", (object?)trabajador.CargoTrabajador ?? DBNull.Value),
                new SqlParameter("@VIP", trabajador.VIP),
                new SqlParameter("@EsAdmin", trabajador.EsAdmin),
                new SqlParameter("@Estado", trabajador.Estado),
                new SqlParameter("@NivelAcceso", trabajador.NivelAcceso),              // NUEVO
                new SqlParameter("@Observaciones", (object?)trabajador.Observaciones ?? DBNull.Value) // NUEVO
            };

            try
            {
                _repoTrabajador.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ModificarTrabajador] Error");
                return false;
            }
        }

        // ===== DELETE =====
        public bool EliminarTrabajador(TrabajadorDto trabajador)
        {
            string sql = "TRA_DEL_Trabajador @idUsuario";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idUsuario", trabajador.IdUsuario)
            };

            try
            {
                _repoTrabajador.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EliminarTrabajador] Error");
                return false;
            }
        }

        // ===== LISTA COMPLETA =====
        public List<TrabajadorDto> GetListaTrabajador()
        {
            string sql = "TRA_LISTADO_Trabajador";
            var lista = _repoTrabajador.GetStoreProcedure(sql);
            return _mapper.Map<List<TrabajadorDto>>(lista);
        }

        // ===== LISTA POR ESTADO / VIGENCIA =====
        public List<TrabajadorDto> GetListaTrabajadorEstado(int IdEmpresa)
        {
            string sql = "TRA_LISTADO_Trabajador_Estado @IdEmpresa";
            var p = new SqlParameter[] { new SqlParameter("@IdEmpresa", IdEmpresa) };
            var lista = _repoTrabajador.GetStoreProcedure(sql, p);
            return _mapper.Map<List<TrabajadorDto>>(lista);
        }

        // ===== OBTENER POR RUT =====
        public TrabajadorDto GetTrabajadorRut(string rut)
        {
            if (string.IsNullOrWhiteSpace(rut)) return null;

            const string sql = "TRA_LIST_Trabajador_RUT @Rut";
            var p = new SqlParameter[] { new SqlParameter("@Rut", rut) };

            try
            {
                var lista = _repoTrabajador.GetStoreProcedure(sql, p);
                var entity = lista?.FirstOrDefault();
                return _mapper.Map<TrabajadorDto>(entity);
            }
            catch (AutoMapperMappingException amex)
            {
                _logger.LogError(amex, "[GetTrabajadorRut] Error de mapeo.");
                return null;
            }
            catch (SqlException sqlex)
            {
                _logger.LogError(sqlex, "[GetTrabajadorRut] Error SQL ({Number})", sqlex.Number);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetTrabajadorRut] Error inesperado.");
                return null;
            }
        }

        // ===== VERIFICA POR RUT (antes “PorNombre”) =====
        public List<TrabajadorDto> VerificaTrabajadorPorNombre(TrabajadorDto trabajador)
        {
            // Ahora validamos por RUT, acorde al rename de la columna
            string sql = "TRA_VERIFICA_TRABAJADOR_RUT @RutTrabajador";
            var p = new SqlParameter[]
            {
                new SqlParameter("@RutTrabajador", (object?)trabajador.RutTrabajador ?? DBNull.Value)
            };

            var lista = _repoTrabajador.GetStoreProcedure(sql, p);
            return _mapper.Map<List<TrabajadorDto>>(lista);
        }

        // ===== VERIFICA POR ID =====
        public List<TrabajadorDto> VerificaTrabajadorPorId(TrabajadorDto trabajador)
        {
            string sql = "TRA_VERIFICA_ID_TRABAJADOR @ID";
            var p = new SqlParameter[] { new SqlParameter("@ID", trabajador.IdUsuario) };
            var lista = _repoTrabajador.GetStoreProcedure(sql, p);
            return _mapper.Map<List<TrabajadorDto>>(lista);
        }

        #endregion
    }
}
