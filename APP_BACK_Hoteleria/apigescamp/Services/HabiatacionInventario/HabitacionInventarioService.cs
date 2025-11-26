using AutoMapper;
using DemoBackend.Dto.Habitacion;
using DemoBackend.Dto.HabitacionInventario;
using DemoBackend.Models.HabitacionInventario;
using DemoBackend.RepositoryGes;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoBackend.Services.HabitacionInventario
{
    public class HabitacionInventarioService : IHabitacionInventarioService
    {
        private readonly IGenericRepositoryEntity<HabitacionInventarioModels> _repo;
        private readonly IMapper _mapper;

        public HabitacionInventarioService(
            IGenericRepositoryEntity<HabitacionInventarioModels> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

              

        public bool CrearHabitacionInsumo(HabitacionInventarioDto dto)
        {
            if (dto == null) return false;
            if (dto.IdHabitacion <= 0 || dto.IdInventario <= 0) return false;

            string sql = "INS_CRE_HabitacionInsumo @idHabitacion,@idInsumo";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idHabitacion", dto.IdHabitacion),
                new SqlParameter("@idInsumo", dto.IdInventario)
                
            };

            try
            {
                _repo.InsertProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        public List<HabitacionInventarioDto> GetListaHabitacionInsumoEstado(int Vigente)
        {
            string sql = "LISTADO_HabitacionInsumo_Estado @Vigencia";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Vigencia", Vigente);

            var listagrupos = _repo.GetStoreProcedure(sql, parametros);


            return _mapper.Map<List<HabitacionInventarioDto>>(listagrupos);
        }

        public bool ModificarHabitacionInsumo(HabitacionInventarioDto dto)
        {
            if (dto == null || dto.IdHabitacionInventario <= 0) return false;

            string sql = "INS_UPD_HabitacionInsumo @idHabitacionInsumo,@idHabitacion,@idInsumo";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idHabitacionInsumo", dto.IdHabitacionInventario),
                new SqlParameter("@idHabitacion", dto.IdHabitacion),
                new SqlParameter("@idInsumo", dto.IdInventario)
              
            };

            try
            {
                _repo.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool EliminarHabitacionInsumo(int idHabitacionInsumo)
        {
            if (idHabitacionInsumo <= 0) return false;

            string sql = "INS_DEL_HabitacionInsumo @idHabitacionInsumo";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idHabitacionInsumo", idHabitacionInsumo),
            };

            try
            {
                _repo.ExecuteProcedure(sql, p);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        
    }
}