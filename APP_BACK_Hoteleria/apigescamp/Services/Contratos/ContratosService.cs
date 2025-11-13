using AutoMapper;
using DemoBackend.Dto.Contratos;
using DemoBackend.Models.Contratos;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Contratos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoBackend.Services
{
    public class ContratosService : IContratosService
    {
        private readonly IGenericRepositoryEntity<ContratosModels> _listaContratos;
        private readonly IGenericRepositoryEntity<ContratoTrabajadoresModels> _listaContratosTrabajadores;
        private readonly IMapper _mapper;

        public ContratosService(
            IGenericRepositoryEntity<ContratosModels> listaContratos,
            IGenericRepositoryEntity<ContratoTrabajadoresModels> listaContratosTrabajadores,
            IMapper mapper)
        {
            _listaContratos = listaContratos;
            _listaContratosTrabajadores = listaContratosTrabajadores;
            _mapper = mapper;
        }

        public List<ContratoDto> GetContratos(string? filtro)
        {
            const string sql = "HOT_CONT_LISTAR @Filtro";
            var p = new SqlParameter[]
            {
                new SqlParameter("@Filtro", (object?)filtro ?? DBNull.Value)
            };

            var lista = _listaContratos.GetStoreProcedure(sql, p);
            var dto = _mapper.Map<List<ContratoDto>>(lista);

            // detalle de trabajadores
            const string sqlDet = "HOT_CONT_LISTAR_TRABAJADORES";
            var det = _listaContratosTrabajadores.GetStoreProcedure(sqlDet);

            foreach (var c in dto)
            {
                c.Trabajadores = det
                    .Where(d => d.IdContrato == c.IdContrato)
                    .Select(d => _mapper.Map<ContratoTrabajadorDto>(d))
                    .ToList();
            }

            return dto;
        }

        public ContratoDto? GetContrato(int idContrato)
        {
            const string sql = "HOT_CONT_OBTENER @IdContrato";
            var p = new SqlParameter[] { new SqlParameter("@IdContrato", idContrato) };

            var lista = _listaContratos.GetStoreProcedure(sql, p);
            var ent = lista.FirstOrDefault();
            if (ent == null) return null;

            var dto = _mapper.Map<ContratoDto>(ent);

            // detalle
            const string sqlDet = "HOT_CONT_LISTAR_TRABAJADORES @IdContrato";
            var det = _listaContratosTrabajadores.GetStoreProcedure(sqlDet, p);
            dto.Trabajadores = _mapper.Map<List<ContratoTrabajadorDto>>(det);

            return dto;
        }

        public bool CrearContrato(ContratoDto dto)
        {
            // IMPORTANTE: añadimos @IdTipoContrato y Estado como bit
            const string sql = @"HOT_CONT_CREAR 
                @IdEmpresa,
                @NumeroContrato,
                @FechaInicio,
                @FechaFin,
                @Valor,
                @IdCampamento,
                @MaximoTrabajadores,
                @Descripcion,
                @IdTipoContrato,
                @Estado";

            var p = new SqlParameter[]
            {
                new SqlParameter("@IdEmpresa",        (object?)dto.IdEmpresa ?? DBNull.Value),
                new SqlParameter("@NumeroContrato",   (object?)dto.NumeroContrato ?? DBNull.Value),
                new SqlParameter("@FechaInicio",      (object?)dto.FechaInicio ?? DBNull.Value),
                new SqlParameter("@FechaFin",         (object?)dto.FechaFin ?? DBNull.Value),
                new SqlParameter("@Valor",            (object?)dto.Valor ?? DBNull.Value),
                new SqlParameter("@IdCampamento",     (object?)dto.IdCampamento ?? DBNull.Value),
                new SqlParameter("@MaximoTrabajadores",(object?)dto.MaximoTrabajadores ?? DBNull.Value),
                new SqlParameter("@Descripcion",      (object?)dto.Descripcion ?? DBNull.Value),
                new SqlParameter("@IdTipoContrato",   (object?)dto.IdTipoContrato ?? DBNull.Value),
                new SqlParameter("@Estado",           dto.Estado)   // bit
            };

            try
            {
                _listaContratos.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool ActualizarContrato(ContratoDto dto)
        {
            // también añadimos @IdTipoContrato
            const string sql = @"HOT_CONT_ACTUALIZAR 
                @IdContrato,
                @IdEmpresa,
                @NumeroContrato,
                @FechaInicio,
                @FechaFin,
                @Valor,
                @IdCampamento,
                @MaximoTrabajadores,
                @Descripcion,
                @IdTipoContrato,
                @Estado";

            var p = new SqlParameter[]
            {
                new SqlParameter("@IdContrato",        dto.IdContrato),
                new SqlParameter("@IdEmpresa",         (object?)dto.IdEmpresa ?? DBNull.Value),
                new SqlParameter("@NumeroContrato",    (object?)dto.NumeroContrato ?? DBNull.Value),
                new SqlParameter("@FechaInicio",       (object?)dto.FechaInicio ?? DBNull.Value),
                new SqlParameter("@FechaFin",          (object?)dto.FechaFin ?? DBNull.Value),
                new SqlParameter("@Valor",             (object?)dto.Valor ?? DBNull.Value),
                new SqlParameter("@IdCampamento",      (object?)dto.IdCampamento ?? DBNull.Value),
                new SqlParameter("@MaximoTrabajadores",(object?)dto.MaximoTrabajadores ?? DBNull.Value),
                new SqlParameter("@Descripcion",       (object?)dto.Descripcion ?? DBNull.Value),
                new SqlParameter("@IdTipoContrato",    (object?)dto.IdTipoContrato ?? DBNull.Value),
                new SqlParameter("@Estado",            dto.Estado)  // bit
            };

            try
            {
                _listaContratos.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool EliminarContrato(int idContrato)
        {
            const string sql = "HOT_CONT_ELIMINAR @IdContrato";
            var p = new SqlParameter[] { new SqlParameter("@IdContrato", idContrato) };

            try
            {
                _listaContratos.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public ContratoKpiDto GetKpi()
        {
            // ahora Estado es bit
            var lista = _listaContratos.GetAll().ToList();
            var det = _listaContratosTrabajadores.GetAll().ToList();

            return new ContratoKpiDto
            {
                ContratosActivos = lista.Count(x => x.Estado == true),
                EmpresasRegistradas = lista.Select(x => x.IdEmpresa).Distinct().Count(),
                TrabajadoresActivos = det.Count(x => x.Estado == "ACTIVO" || x.Estado == "Activo"),
                VencenPronto = lista.Count(x => x.FechaFin.HasValue)
            };
        }
    }
}
