using AutoMapper;
using DemoBackend.Dto.Insumos;
using DemoBackend.Models.Insumos;
using DemoBackend.RepositoryGes;

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace DemoBackend.Services
{
    public class InsumoService : IInsumoService
    {
        private readonly IGenericRepositoryEntity<InsumoModels> _listaInsumo;
        private readonly IMapper _mapper;

        public InsumoService(
            IGenericRepositoryEntity<InsumoModels> listaInsumo,
            IMapper mapper)
        {
            _listaInsumo = listaInsumo;
            _mapper = mapper;
        }

        #region Insumos

        public bool CrearInsumo(InsumoDto insumo)
        {
            // INS_CRE_Insumo @NombreInsumo, @StockMinimo, @idBodega
            string sql = "INS_CRE_Insumo @NombreInsumo,@StockMinimo,@idBodega";

            var parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@NombreInsumo", (object?)insumo.NombreInsumo ?? DBNull.Value);
            parametros[1] = new SqlParameter("@StockMinimo", (object?)insumo.StockMinimo ?? DBNull.Value);
            parametros[2] = new SqlParameter("@idBodega", (object?)insumo.IdBodega ?? DBNull.Value);

            try
            {
                _listaInsumo.InsertProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }

        public bool ModificarInsumo(InsumoDto insumo)
        {
            // INS_UPD_Insumo @idInsumo, @NombreInsumo, @StockMinimo, @idBodega
            string sql = "INS_UPD_Insumo @idInsumo,@NombreInsumo,@StockMinimo,@idBodega";

            var parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@idInsumo", insumo.IdInsumo);
            parametros[1] = new SqlParameter("@NombreInsumo", (object?)insumo.NombreInsumo ?? DBNull.Value);
            parametros[2] = new SqlParameter("@StockMinimo", (object?)insumo.StockMinimo ?? DBNull.Value);
            parametros[3] = new SqlParameter("@idBodega", (object?)insumo.IdBodega ?? DBNull.Value);

            try
            {
                _listaInsumo.ExecuteProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }

        public bool EliminarInsumo(InsumoDto insumo)
        {
            // INS_DEL_Insumo @idInsumo
            string sql = "INS_DEL_Insumo @idInsumo";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@idInsumo", insumo.IdInsumo);

            try
            {
                _listaInsumo.ExecuteProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }

        public List<InsumoDto> GetListaInsumo()
        {
            // LISTADO_Insumo
            string sql = "LISTADO_Insumo";
            var lista = _listaInsumo.GetStoreProcedure(sql);
            return _mapper.Map<List<InsumoDto>>(lista);
        }

        public List<InsumoDto> GetListaInsumoEstado(int vigencia)
        {
            // LISTADO_Insumo_Estado @Vigencia
            string sql = "LISTADO_Insumo_Estado @Vigencia";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Vigencia", vigencia);

            var lista = _listaInsumo.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<InsumoDto>>(lista);
        }

        public List<InsumoDto> VerificaInsumoPorId(InsumoDto insumo)
        {
            // INS_VERIFICA_ID_INSUMO @idInsumo
            string sql = "INS_VERIFICA_ID_INSUMO @idInsumo";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@idInsumo", insumo.IdInsumo);

            var lista = _listaInsumo.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<InsumoDto>>(lista);
        }

        public List<InsumoDto> BuscaInsumos(InsumoDto insumo)
        {
            // INS_BUSCA_Insumo @idInsumo, @NombreInsumo, @StockMinimo, @idBodega
            string sql = "INS_BUSCA_Insumo @idInsumo, @NombreInsumo, @StockMinimo, @idBodega";

            var parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@idInsumo", insumo.IdInsumo);
            parametros[1] = new SqlParameter("@NombreInsumo", (object?)insumo.NombreInsumo ?? DBNull.Value);
            parametros[2] = new SqlParameter("@StockMinimo", (object?)insumo.StockMinimo ?? DBNull.Value);
            parametros[3] = new SqlParameter("@idBodega", (object?)insumo.IdBodega ?? DBNull.Value);

            var lista = _listaInsumo.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<InsumoDto>>(lista);
        }

        #endregion
    }
}
