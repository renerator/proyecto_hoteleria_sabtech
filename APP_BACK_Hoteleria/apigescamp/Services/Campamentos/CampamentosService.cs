using AutoMapper;
using DemoBackend.Dto.Campamentos;
using DemoBackend.Dto.Reserva;
using DemoBackend.Models.Campamentos;
using DemoBackend.Models.Reserva;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Campamentos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DemoBackend.Services
{
    public class CampamentosService : ICampamentosService
    {
        private readonly IGenericRepositoryEntity<CampamentosModels> _listaCampamentos;
        private readonly IGenericRepositoryEntity<CampamentoAreasModels> _listaAreas;
        private readonly IGenericRepositoryEntity<CampamentoKPIModels> _listaKPI;
        private readonly IMapper _mapper;
        //cambio 1-12
        public CampamentosService(
            IGenericRepositoryEntity<CampamentosModels> listaCampamentos,
            IGenericRepositoryEntity<CampamentoAreasModels> listaAreas,
            IGenericRepositoryEntity<CampamentoKPIModels> listaKPI,
            IMapper mapper)
        {
            _listaCampamentos = listaCampamentos;
            _listaAreas = listaAreas;
            _listaKPI = listaKPI;
            _mapper = mapper;
        }

        public List<CampamentoDto> GetCampamentos()
        {
            const string sql = "HOT_CAMP_LISTAR";
            var lista = _listaCampamentos.GetStoreProcedure(sql);
            var dto = _mapper.Map<List<CampamentoDto>>(lista);

            // áreas
            const string sqlAreas = "HOT_CAMP_LISTAR_AREAS";
            var areas = _listaAreas.GetStoreProcedure(sqlAreas);

            foreach (var c in dto)
            {
                c.Areas = areas
                    .Where(a => a.IdCampamento == c.IdCampamento)
                    .Select(a => _mapper.Map<CampamentoAreaDto>(a))
                    .ToList();
            }

            return dto;
        }

        public CampamentoDto? GetCampamento(int idCampamento)
        {
            const string sql = "HOT_CAMP_OBTENER @IdCampamento";
            var p = new SqlParameter[] { new SqlParameter("@IdCampamento", idCampamento) };
            var lista = _listaCampamentos.GetStoreProcedure(sql, p);
            var ent = lista.FirstOrDefault();
            if (ent == null) return null;

            var dto = _mapper.Map<CampamentoDto>(ent);

            const string sqlAreas = "HOT_CAMP_LISTAR_AREAS @IdCampamento";
            var areas = _listaAreas.GetStoreProcedure(sqlAreas, p);
            dto.Areas = _mapper.Map<List<CampamentoAreaDto>>(areas);
            return dto;
        }

        public bool CrearCampamento(CampamentoDto dto)
        {
            const string sql = "HOT_CAMP_CREAR @Nombre,@Codigo,@Ubicacion,@Capacidad,@Estado,@Encargado,@Descripcion";
            var p = new SqlParameter[]
            {
                new SqlParameter("@Nombre", (object?)dto.Nombre ?? DBNull.Value),
                new SqlParameter("@Codigo", (object?)dto.Codigo ?? DBNull.Value),
                new SqlParameter("@Ubicacion", (object?)dto.Ubicacion ?? DBNull.Value),
                new SqlParameter("@Capacidad", dto.Capacidad),
                new SqlParameter("@Estado", (object?)dto.Estado ?? DBNull.Value),
                new SqlParameter("@Encargado", (object?)dto.Encargado ?? DBNull.Value),
                new SqlParameter("@Descripcion", (object?)dto.Descripcion ?? DBNull.Value),
            };

            try
            {
                _listaCampamentos.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool ActualizarCampamento(CampamentoDto dto)
        {
            const string sql = "HOT_CAMP_ACTUALIZAR @IdCampamento,@Nombre,@Codigo,@Ubicacion,@Capacidad,@Estado,@Encargado,@Descripcion";
            var p = new SqlParameter[]
            {
                new SqlParameter("@IdCampamento", dto.IdCampamento),
                new SqlParameter("@Nombre", (object?)dto.Nombre ?? DBNull.Value),
                new SqlParameter("@Codigo", (object?)dto.Codigo ?? DBNull.Value),
                new SqlParameter("@Ubicacion", (object?)dto.Ubicacion ?? DBNull.Value),
                new SqlParameter("@Capacidad", dto.Capacidad),
                new SqlParameter("@Estado", (object?)dto.Estado ?? DBNull.Value),
                new SqlParameter("@Encargado", (object?)dto.Encargado ?? DBNull.Value),
                new SqlParameter("@Descripcion", (object?)dto.Descripcion ?? DBNull.Value),
            };

            try
            {
                _listaCampamentos.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool EliminarCampamento(int idCampamento)
        {
            const string sql = "HOT_CAMP_ELIMINAR @IdCampamento";
            var p = new SqlParameter[] { new SqlParameter("@IdCampamento", idCampamento) };

            try
            {
                _listaCampamentos.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public CampamentoKpiDto GetKpi()
        {
            

            //var hoy = DateTime.Today;
            //var d = (desde ?? hoy).Date;
            //var h = (hasta ?? hoy).Date;

            // Nombre del SP correcto
            string sql = "HOT_DASH_Campamentos";
            var parametros = new SqlParameter[0];
           

            var dto = new CampamentoKpiDto();

            try
            {
                var kpiRow = _listaKPI.GetStoreProcedure(sql, parametros).FirstOrDefault();
                if (kpiRow != null)
                {
                    dto.CampamentosActivos = kpiRow.CampamentosActivos;
                    dto.AreasComunes = kpiRow.AreasComunes;
                    dto.Habitaciones = kpiRow.Habitaciones;
                    dto.TasaUtilizacion = kpiRow.TasaUtilizacion;


                }



            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerDashboard: {ex.Message}");
            }

            return dto;
        }

        // Services/CampamentosService.cs (agregar método)
        public List<CampamentoDto> ListarCombo(bool? soloActivos, string? filtro)
        {
            const string sql = "HOT_CAMP_COMBO @SoloActivos,@Filtro";
            var p = new[]
            {
        new SqlParameter("@SoloActivos", (object?)soloActivos ?? DBNull.Value),
        new SqlParameter("@Filtro",      (object?)filtro ?? DBNull.Value)
    };

            var lista = _listaCampamentos.GetStoreProcedure(sql, p);

            // devolvemos solo lo necesario para el combo
            return lista.Select(c => new CampamentoDto
            {
                IdCampamento = c.IdCampamento,
                Nombre = c.Nombre,
                Codigo = c.Codigo
            })
                    .OrderBy(x => x.Nombre)
                    .ToList();
        }


        // podrías tener un SP extra, aquí lo calculo con lo que hay

    }

}
