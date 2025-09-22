using AutoMapper;
using DemoBackend.Dto.Mantenedores;
using DemoBackend.Models.Mantenedores;
using DemoBackend.RepositoryGes;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace DemoBackend.Services.Mantenedores
{
    public class MantenedoresService : IMantenedoresService
    {
        private readonly IGenericRepositoryEntity<AreasModels> _listaAreas;
        private readonly IMapper _mapper;

        public MantenedoresService(
            IGenericRepositoryEntity<AreasModels> listaAreas,
            IMapper mapper)
        {
            _listaAreas = listaAreas;
            _mapper = mapper;
        }

        #region Area
        public bool CrearAreas(AreasDto areasModels)
        {
            string sql = "MAN_CRE_AREAS @NOMBRE_AREA";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@NOMBRE_AREA", areasModels.NombreArea);

            var gr = false;
            try
            {
                _listaAreas.InsertProcedure(sql, parametros);
                gr = true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return gr;
        }

        public bool ModificarAreas(AreasDto areasModels)
        {
            string sql = "MAN_UPD_AREAS @NOMBRE_AREA,@ID_AREA";
            var parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@NOMBRE_AREA", areasModels.NombreArea);
            parametros[1] = new SqlParameter("@ID_AREA", areasModels.IdArea);

            var gr = false;
            try
            {
                _listaAreas.ExecuteProcedure(sql, parametros);
                gr = true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return gr;
        }

        public bool EliminarAreas(AreasDto areasModels)
        {
            string sql = "MAN_DEL_AREAS @ID_AREA";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@ID_AREA", areasModels.IdArea);

            var gr = false;
            try
            {
                _listaAreas.ExecuteProcedure(sql, parametros);
                gr = true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return gr;
        }

        public List<AreasDto> GetListaAreas()
        {
            string sql = "LISTADO_AREAS";

            List<AreasDto> gr = new List<AreasDto>();
            var listagrupos = _listaAreas.GetStoreProcedure(sql);
            foreach (var item in listagrupos)
            {
                AreasDto resu = new AreasDto();
                resu.IdArea = item.idArea;
                resu.NombreArea = item.NombreArea;
                gr.Add(resu);
            }
            return gr;
        }

        public List<AreasDto> GetListaAreasEstado(int Vigente)
        {
            string sql = "LISTADO_AREAS_Estado @Vigencia";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Vigencia", Vigente);

            List<AreasDto> gr = new List<AreasDto>();
            var listagrupos = _listaAreas.GetStoreProcedure(sql, parametros);
            foreach (var item in listagrupos)
            {
                AreasDto resu = new AreasDto();
                resu.IdArea = item.idArea;
                resu.NombreArea = item.NombreArea;
                gr.Add(resu);
            }
            return gr;
        }

        public List<AreasDto> VerificaArea(AreasDto areasModels)
        {
            string sql = "MAN_VERIFICA_AREA @NombreArea";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@NombreArea", areasModels.NombreArea);

            List<AreasDto> gr = new List<AreasDto>();
            var listagrupos = _listaAreas.GetStoreProcedure(sql, parametros);
            foreach (var item in listagrupos)
            {
                AreasDto resu = new AreasDto();
                resu.IdArea = item.idArea;
                resu.NombreArea = item.NombreArea;
                gr.Add(resu);
            }
            return gr;
        }

        public List<AreasDto> VerificaAreaId(AreasDto areasModels)
        {
            string sql = "MAN_VERIFICA_ID_AREA @ID";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@ID", areasModels.IdArea);

            List<AreasDto> gr = new List<AreasDto>();
            var listagrupos = _listaAreas.GetStoreProcedure(sql, parametros);
            foreach (var item in listagrupos)
            {
                AreasDto resu = new AreasDto();
                resu.IdArea = item.idArea;
                resu.NombreArea = item.NombreArea;
                gr.Add(resu);
            }
            return gr;
        }
        #endregion
    }
}

