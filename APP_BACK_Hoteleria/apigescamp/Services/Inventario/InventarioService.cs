using AutoMapper;
using DemoBackend.Dto.Inventario;
using DemoBackend.Models.Inventario;
using DemoBackend.RepositoryGes;
using DemoBackend.Services.Inventario;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DemoBackend.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly IGenericRepositoryEntity<InventarioModels> _listaInventario;
        private readonly IGenericRepositoryEntity<InventarioMovimientosModels> _listaMovimientos;
        private readonly IMapper _mapper;

        public InventarioService(
            IGenericRepositoryEntity<InventarioModels> listaInventario,
            IGenericRepositoryEntity<InventarioMovimientosModels> listaMovimientos,
            IMapper mapper)
        {
            _listaInventario = listaInventario;
            _listaMovimientos = listaMovimientos;
            _mapper = mapper;
        }

        public List<InventarioItemDto> GetInventario(string? criterio, string? categoria, string? estado, string? habitacion)
        {
            const string sql = "HOT_INV_LISTAR @Criterio,@Categoria,@Estado,@Habitacion";
            var p = new SqlParameter[]
            {
                new SqlParameter("@Criterio", (object?)criterio ?? DBNull.Value),
                new SqlParameter("@Categoria", (object?)categoria ?? DBNull.Value),
                new SqlParameter("@Estado", (object?)estado ?? DBNull.Value),
                new SqlParameter("@Habitacion", (object?)habitacion ?? DBNull.Value)
            };

            var lista = _listaInventario.GetStoreProcedure(sql, p);
            return _mapper.Map<List<InventarioItemDto>>(lista);
        }

        public InventarioItemDto? GetItem(int idArticulo)
        {
            const string sql = "HOT_INV_OBTENER @IdArticulo";
            var p = new SqlParameter[] { new SqlParameter("@IdArticulo", idArticulo) };

            var lista = _listaInventario.GetStoreProcedure(sql, p);
            var ent = lista.FirstOrDefault();
            return ent == null ? null : _mapper.Map<InventarioItemDto>(ent);
        }

        public bool CrearItem(InventarioItemDto dto)
        {
            const string sql = "HOT_INV_CREAR @Nombre,@Categoria,@Habitacion,@Estado,@Valor,@Marca,@Modelo,@Serie,@Observaciones,@FotoUrl";
            var p = new SqlParameter[]
            {
                
                new SqlParameter("@Nombre", (object?)dto.Nombre ?? DBNull.Value),
                new SqlParameter("@Categoria", (object?)dto.Categoria ?? DBNull.Value),
                new SqlParameter("@Habitacion", (object?)dto.Habitacion ?? DBNull.Value),
                new SqlParameter("@Estado", (object?)dto.Estado ?? DBNull.Value),
                new SqlParameter("@Valor", (object?)dto.Valor ?? DBNull.Value),
                new SqlParameter("@Marca", (object?)dto.Marca ?? DBNull.Value),
                new SqlParameter("@Modelo", (object?)dto.Modelo ?? DBNull.Value),
                new SqlParameter("@Serie", (object?)dto.Serie ?? DBNull.Value),
                new SqlParameter("@Observaciones", (object?)dto.Observaciones ?? DBNull.Value),
                new SqlParameter("@FotoUrl", (object?)dto.FotoUrl ?? DBNull.Value)
            };

            try
            {
                _listaInventario.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool ActualizarItem(InventarioItemDto dto)
        {
            const string sql = "HOT_INV_ACTUALIZAR @IdArticulo,@Nombre,@Categoria,@Habitacion,@Estado,@Valor,@Marca,@Modelo,@Serie,@Observaciones,@FotoUrl";
            var p = new SqlParameter[]
            {
                new SqlParameter("@IdArticulo", dto.IdArticulo),
                new SqlParameter("@Nombre", (object?)dto.Nombre ?? DBNull.Value),
                new SqlParameter("@Categoria", (object?)dto.Categoria ?? DBNull.Value),
                new SqlParameter("@Habitacion", (object?)dto.Habitacion ?? DBNull.Value),
                new SqlParameter("@Estado", (object?)dto.Estado ?? DBNull.Value),
                new SqlParameter("@Valor", (object?)dto.Valor ?? DBNull.Value),
                new SqlParameter("@Marca", (object?)dto.Marca ?? DBNull.Value),
                new SqlParameter("@Modelo", (object?)dto.Modelo ?? DBNull.Value),
                new SqlParameter("@Serie", (object?)dto.Serie ?? DBNull.Value),
                new SqlParameter("@Observaciones", (object?)dto.Observaciones ?? DBNull.Value),
                new SqlParameter("@FotoUrl", (object?)dto.FotoUrl ?? DBNull.Value)
            };

            try
            {
                _listaInventario.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool EliminarItem(int idArticulo)
        {
            const string sql = "HOT_INV_ELIMINAR @IdArticulo";
            var p = new SqlParameter[] { new SqlParameter("@IdArticulo", idArticulo) };

            try
            {
                _listaInventario.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public List<InventarioMovimientoPostDto> GetMovimientos(int idArticulo)
        {
            const string sql = "HOT_INV_LISTAR_MOV @IdArticulo";
            var p = new SqlParameter[] { new SqlParameter("@IdArticulo", idArticulo) };

            var lista = _listaMovimientos.GetStoreProcedure(sql, p);
            return lista.Select(m => new InventarioMovimientoPostDto
            {
                IdArticulo = m.IdArticulo,
                TipoMovimiento = m.TipoMovimiento,
                HabitacionDesde = m.HabitacionDesde,
                HabitacionHasta = m.HabitacionHasta,
                FechaMovimiento = m.FechaMovimiento,
                Responsable = m.Responsable,
                Motivo = m.Motivo
            }).ToList();
        }

        public InventarioKpiDto GetKpi()
        {
            // si no hay SP de KPI, lo armamos rápido
            var all = _listaInventario.GetAll().ToList();
            return new InventarioKpiDto
            {
                TotalItems = all.Count,
                Disponibles = all.Count(x => x.Estado == "disponible"),
                Faltantes = all.Count(x => x.Estado == "faltante"),
                EnMantenimiento = all.Count(x => x.Estado == "mantenimiento")
            };
        }
    }
}
