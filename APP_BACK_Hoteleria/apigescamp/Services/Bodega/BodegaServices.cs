using AutoMapper;
using DemoBackend.Dto.Bodega;
using DemoBackend.Dto.Habitacion;
using DemoBackend.Models.Bodega;
using DemoBackend.RepositoryGes;

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace DemoBackend.Services
{
    public class BodegaService : IBodegaService
    {
        private readonly IGenericRepositoryEntity<BodegaModels> _listaBodega;
        private readonly IMapper _mapper;

        public BodegaService(
            IGenericRepositoryEntity<BodegaModels> listaBodega,
            IMapper mapper)
        {
            _listaBodega = listaBodega;
            _mapper = mapper;
        }

        #region Bodegas

        public bool CrearBodega(BodegaDto bodega)
        {
            // BOD_CRE_Bodega @NombreBodega,@Ubicacion,@idEmpresa
            string sql = "BOD_CRE_Bodega @NombreBodega,@Ubicacion,@idEmpresa";
            var parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("@NombreBodega", (object?)bodega.NombreBodega ?? DBNull.Value);
            parametros[1] = new SqlParameter("@Ubicacion", (object?)bodega.Ubicacion ?? DBNull.Value);
            parametros[2] = new SqlParameter("@idEmpresa", (object?)bodega.IdEmpresa ?? DBNull.Value);

            try
            {
                _listaBodega.InsertProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool ModificarBodega(BodegaDto bodega)
        {
            // BOD_UPD_Bodega @idBodega,@NombreBodega,@Ubicacion,@idEmpresa
            string sql = "BOD_UPD_Bodega @idBodega,@NombreBodega,@Ubicacion,@idEmpresa";
            var parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@idBodega", bodega.IdBodega);
            parametros[1] = new SqlParameter("@NombreBodega", (object?)bodega.NombreBodega ?? DBNull.Value);
            parametros[2] = new SqlParameter("@Ubicacion", (object?)bodega.Ubicacion ?? DBNull.Value);
            parametros[3] = new SqlParameter("@idEmpresa", (object?)bodega.IdEmpresa ?? DBNull.Value);

            try
            {
                _listaBodega.ExecuteProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool EliminarBodega(BodegaDto bodega)
        {
            // BOD_DEL_Bodega @idBodega
            string sql = "BOD_DEL_Bodega @idBodega";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@idBodega", bodega.IdBodega);

            try
            {
                _listaBodega.ExecuteProcedure(sql, parametros);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public List<BodegaDto> GetListaBodega()
        {
            string sql = "LISTADO_Bodega";
            var lista = _listaBodega.GetStoreProcedure(sql);
            return _mapper.Map<List<BodegaDto>>(lista);
        }

        public List<BodegaDto> GetListaBodegaEmpresa(int idEmpresa)
        {
            string sql = "LISTADO_Bodega_Empresa @idEmpresa";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@idEmpresa", idEmpresa);

            var lista = _listaBodega.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<BodegaDto>>(lista);
        }

        public List<BodegaDto> VerificaBodegaPorId(BodegaDto bodega)
        {
            string sql = "BOD_VERIFICA_ID_BODEGA @idBodega";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@idBodega", bodega.IdBodega);

            var lista = _listaBodega.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<BodegaDto>>(lista);
        }

        public List<BodegaDto> BuscaBodegas(BodegaDto bodega)
        {
            // BOD_BUSCA_Bodega @idBodega,@NombreBodega,@Ubicacion,@idEmpresa
            string sql = "BOD_BUSCA_Bodega @idBodega,@NombreBodega,@Ubicacion,@idEmpresa";
            var parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@idBodega", bodega.IdBodega);
            parametros[1] = new SqlParameter("@NombreBodega", (object?)bodega.NombreBodega ?? DBNull.Value);
            parametros[2] = new SqlParameter("@Ubicacion", (object?)bodega.Ubicacion ?? DBNull.Value);
            parametros[3] = new SqlParameter("@idEmpresa", (object?)bodega.IdEmpresa ?? DBNull.Value);

            var lista = _listaBodega.GetStoreProcedure(sql, parametros);
            return _mapper.Map<List<BodegaDto>>(lista);
        }

        List<BodegaDto> IBodegaService.GetListaBodegaEstado(int vigencia)
        {
            string sql = "LISTADO_Bodega";
            var listagrupos = _listaBodega.GetStoreProcedure(sql);


            return _mapper.Map<List<BodegaDto>>(listagrupos);
        }



        #endregion
    }
}
