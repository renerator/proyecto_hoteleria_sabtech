using AutoMapper;
using DemoBackend.Dto.Trabajador;
using DemoBackend.Models.Trabajador;                 // <- contiene la entidad Trabajador
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Habitacion;
using DemoBackend.Services.Trabajador;    // <- contiene la interfaz ITrabajadorService
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using static DemoBackend.Models.Trabajador.TrabajadorModels;

namespace DemoBackend.Services
{
    public class TrabajadorService : ITrabajadorService
    {
        private readonly IGenericRepositoryEntity<TrabajadorModels> _repoTrabajador;
        private readonly IMapper _mapper;

        public TrabajadorService(
            IGenericRepositoryEntity<TrabajadorModels> repoTrabajador,
            IMapper mapper)
        {
            _repoTrabajador = repoTrabajador;
            _mapper = mapper;
        }

        #region Trabajador (CRUD + verificaciones + listados)

        // CREATE
        public bool CrearTrabajador(TrabajadorDto trabajador)
        {
            // TODO: Ajusta el nombre del SP si es diferente
            string sql = "TRA_CRE_Trabajador " +
                         "@idEmpresaContratista," +
                         "@DNITrabajador," +
                         "@NombresTrabajador," +
                         "@PaternoTrabajador," +
                         "@MaternoTrabajador," +
                         "@EmailTrabajador," +
                         "@CargoTrabajador," +
                         "@VIP," +
                         "@EsAdmin," +
                         "@Estado";

            var p = new SqlParameter[10];
            p[0] = new SqlParameter("@idEmpresaContratista", trabajador.IdEmpresaContratista);
            p[1] = new SqlParameter("@DNITrabajador", (object?)trabajador.DNITrabajador ?? DBNull.Value);
            p[2] = new SqlParameter("@NombresTrabajador", (object?)trabajador.NombresTrabajador ?? DBNull.Value);
            p[3] = new SqlParameter("@PaternoTrabajador", (object?)trabajador.PaternoTrabajador ?? DBNull.Value);
            p[4] = new SqlParameter("@MaternoTrabajador", (object?)trabajador.MaternoTrabajador ?? DBNull.Value);
            p[5] = new SqlParameter("@EmailTrabajador", (object?)trabajador.EmailTrabajador ?? DBNull.Value);
            p[6] = new SqlParameter("@CargoTrabajador", (object?)trabajador.CargoTrabajador ?? DBNull.Value);
            p[7] = new SqlParameter("@VIP", trabajador.VIP);
            p[8] = new SqlParameter("@EsAdmin", trabajador.EsAdmin);
            p[9] = new SqlParameter("@Estado", trabajador.Estado);

            try
            {
                _repoTrabajador.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }

        // UPDATE
        public bool ModificarTrabajador(TrabajadorDto trabajador)
        {
            // TODO: Ajusta el nombre del SP si es diferente
            string sql = "TRA_UPD_Trabajador " +
                         "@idTrabajador," +
                         "@idEmpresaContratista," +
                         "@DNITrabajador," +
                         "@NombresTrabajador," +
                         "@PaternoTrabajador," +
                         "@MaternoTrabajador," +
                         "@EmailTrabajador," +
                         "@CargoTrabajador," +
                         "@VIP," +
                         "@EsAdmin," +
                         "@Estado";

            var p = new SqlParameter[11];
            p[0] = new SqlParameter("@idTrabajador", trabajador.IdTrabajador);
            p[1] = new SqlParameter("@idEmpresaContratista", trabajador.IdEmpresaContratista);
            p[2] = new SqlParameter("@DNITrabajador", (object?)trabajador.DNITrabajador ?? DBNull.Value);
            p[3] = new SqlParameter("@NombresTrabajador", (object?)trabajador.NombresTrabajador ?? DBNull.Value);
            p[4] = new SqlParameter("@PaternoTrabajador", (object?)trabajador.PaternoTrabajador ?? DBNull.Value);
            p[5] = new SqlParameter("@MaternoTrabajador", (object?)trabajador.MaternoTrabajador ?? DBNull.Value);
            p[6] = new SqlParameter("@EmailTrabajador", (object?)trabajador.EmailTrabajador ?? DBNull.Value);
            p[7] = new SqlParameter("@CargoTrabajador", (object?)trabajador.CargoTrabajador ?? DBNull.Value);
            p[8] = new SqlParameter("@VIP", trabajador.VIP);
            p[9] = new SqlParameter("@EsAdmin", trabajador.EsAdmin);
            p[10] = new SqlParameter("@Estado", trabajador.Estado);

            try
            {
                _repoTrabajador.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }

        // DELETE
        public bool EliminarTrabajador(TrabajadorDto trabajador)
        {
            // TODO: Ajusta el nombre del SP si es diferente
            string sql = "TRA_DEL_Trabajador @idTrabajador";
            var p = new SqlParameter[1];
            p[0] = new SqlParameter("@idTrabajador", trabajador.IdTrabajador);

            try
            {
                _repoTrabajador.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }

        // LISTA COMPLETA
        public List<TrabajadorDto> GetListaTrabajador()
        {
            // TODO: Ajusta el nombre del SP si es diferente
            string sql = "TRA_LISTADO_Trabajador";
            var lista = _repoTrabajador.GetStoreProcedure(sql);
            return _mapper.Map<List<TrabajadorDto>>(lista);
        }

        // LISTA POR ESTADO / VIGENCIA
        public List<TrabajadorDto> GetListaTrabajadorEstado(int vigencia)
        {
           
            string sql = "TRA_LISTADO_Trabajador_Estado @Vigencia";
            var p = new SqlParameter[1];
            p[0] = new SqlParameter("@Vigencia", vigencia);

            var lista = _repoTrabajador.GetStoreProcedure(sql, p);
            return _mapper.Map<List<TrabajadorDto>>(lista);
        }

        // VERIFICA POR "NOMBRE" (puedes validar por DNI si es único)
        public List<TrabajadorDto> VerificaTrabajadorPorNombre(TrabajadorDto trabajador)
        {
           
            string sql = "TRA_VERIFICA_TRABAJADOR @DNITrabajador";
            var p = new SqlParameter[1];
            p[0] = new SqlParameter("@DNITrabajador", (object?)trabajador.DNITrabajador ?? DBNull.Value);

            var lista = _repoTrabajador.GetStoreProcedure(sql, p);
            return _mapper.Map<List<TrabajadorDto>>(lista);
        }

        // VERIFICA POR ID
        public List<TrabajadorDto> VerificaTrabajadorPorId(TrabajadorDto trabajador)
        {
           
            string sql = "TRA_VERIFICA_ID_TRABAJADOR @ID";
            var p = new SqlParameter[1];
            p[0] = new SqlParameter("@ID", trabajador.IdTrabajador);

            var lista = _repoTrabajador.GetStoreProcedure(sql, p);
            return _mapper.Map<List<TrabajadorDto>>(lista);
        }

        #endregion
    }
}
