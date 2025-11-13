// DemoBackend/Services/EmpresaService.cs
using AutoMapper;
using DemoBackend.Dto.Empresa;
using DemoBackend.Models.EmpresaContratista; // <--- Ajusta al namespace real del modelo
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Empresa;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DemoBackend.Services.Empresa
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IGenericRepositoryEntity<EmpresaContratistaModels> _repoEmpresas;
        private readonly IMapper _mapper;

        public EmpresaService(
            IGenericRepositoryEntity<EmpresaContratistaModels> repoEmpresas,
            IMapper mapper)
        {
            _repoEmpresas = repoEmpresas;
            _mapper = mapper;
        }

        /// <summary>
        /// Devuelve lista para combo desde admin_EmpresaContratista.
        /// SP: ADMIN_EMPCTR_LISTAR @SoloActivas,@Filtro
        /// </summary>
        public List<EmpresaDto> Listar(bool? soloActivas, string? filtro)
        {
            const string sql = "ADMIN_EMPCTR_LISTAR @SoloActivas,@Filtro";
            var p = new SqlParameter[]
            {
                new SqlParameter("@SoloActivas", (object?)soloActivas ?? DBNull.Value),
                new SqlParameter("@Filtro", (object?)filtro ?? DBNull.Value)
            };

            var lista = _repoEmpresas.GetStoreProcedure(sql, p);

            // Mapeo manual para asegurar nombres (tabla: admin_EmpresaContratista)
            // idEmpresaContratista -> IdEmpresa ; NombreEmpresaContratista -> Nombre
            return lista.Select(e => new EmpresaDto
            {
                IdEmpresa = e.idEmpresaContratista,
                Nombre = e.NombreEmpresaContratista,
                Rut=e.DNIEmpresaContratista,
            })
            .OrderBy(x => x.Nombre)
            .ToList();
        }

        /// <summary>
        /// Inserta una empresa en admin_EmpresaContratista.
        /// SP: ADMIN_EMPCTR_CREAR @NombreEmpresaContratista,@DNIEmpresaContratista,@DireccionEmpresaContratista,
        ///                         @idPais,@idEmpresa,@Estado,@TelefonoEmpresa,@EmailEmpresa,@ContactoPrincipal,@DescripcionEmpresa
        /// </summary>
        public bool Crear(EmpresaCrearDto dto)
        {
            const string sql = "ADMIN_EMPCTR_CREAR " +
                               "@NombreEmpresaContratista,@DNIEmpresaContratista,@DireccionEmpresaContratista," +
                               "@idPais,@idEmpresa,@Estado,@TelefonoEmpresa,@EmailEmpresa,@ContactoPrincipal,@DescripcionEmpresa";

            var p = new SqlParameter[]
            {
                new SqlParameter("@NombreEmpresaContratista", (object?)dto.NombreEmpresaContratista ?? DBNull.Value),
                new SqlParameter("@DNIEmpresaContratista", (object?)dto.DNIEmpresaContratista ?? DBNull.Value),
                new SqlParameter("@DireccionEmpresaContratista", (object?)dto.DireccionEmpresaContratista ?? DBNull.Value),
                new SqlParameter("@idPais", (object?)dto.idPais ?? DBNull.Value),
                new SqlParameter("@idEmpresa", (object?)dto.idEmpresa ?? DBNull.Value),
                new SqlParameter("@Estado", (object?)dto.Estado ?? DBNull.Value),
                new SqlParameter("@TelefonoEmpresa", (object?)dto.TelefonoEmpresa ?? DBNull.Value),
                new SqlParameter("@EmailEmpresa", (object?)dto.EmailEmpresa ?? DBNull.Value),
                new SqlParameter("@ContactoPrincipal", (object?)dto.ContactoPrincipal ?? DBNull.Value),
                new SqlParameter("@DescripcionEmpresa", (object?)dto.DescripcionEmpresa ?? DBNull.Value)
            };

            try
            {
                _repoEmpresas.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmpresaService.Crear] {ex}");
                return false;
            }
        }
    }
}
