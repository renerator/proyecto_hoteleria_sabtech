using AutoMapper;
using DemoBackend.Dto.Habitacion;
using DemoBackend.Dto.HabitacionInsumo;
using DemoBackend.Models.HabitacionInsumo;
using DemoBackend.RepositoryGes;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoBackend.Services.HabitacionInsumo
{
    public class HabitacionInsumoService : IHabitacionInsumoService
    {
        private readonly IGenericRepositoryEntity<HabitacionInsumoModels> _repo;
        private readonly IMapper _mapper;

        public HabitacionInsumoService(
            IGenericRepositoryEntity<HabitacionInsumoModels> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

              

        public bool CrearHabitacionInsumo(HabitacionInsumoDto dto)
        {
            if (dto == null) return false;
            if (dto.idHabitacion <= 0 || dto.idInsumo <= 0) return false;

            string sql = "INS_CRE_HabitacionInsumo @idHabitacion,@idInsumo";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idHabitacion", dto.idHabitacion),
                new SqlParameter("@idInsumo", dto.idInsumo)
                
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
        public List<HabitacionInsumoDto> GetListaHabitacionInsumoEstado(int Vigente)
        {
            string sql = "LISTADO_HabitacionInsumo_Estado @Vigencia";
            var parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Vigencia", Vigente);

            var listagrupos = _repo.GetStoreProcedure(sql, parametros);


            return _mapper.Map<List<HabitacionInsumoDto>>(listagrupos);
        }

        public bool ModificarHabitacionInsumo(HabitacionInsumoDto dto)
        {
            if (dto == null || dto.idHabitacionInsumo <= 0) return false;

            string sql = "INS_UPD_HabitacionInsumo @idHabitacionInsumo,@idHabitacion,@idInsumo";
            var p = new SqlParameter[]
            {
                new SqlParameter("@idHabitacionInsumo", dto.idHabitacionInsumo),
                new SqlParameter("@idHabitacion", dto.idHabitacion),
                new SqlParameter("@idInsumo", dto.idInsumo)
              
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