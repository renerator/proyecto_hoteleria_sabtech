using AutoMapper;
using Microsoft.Data.SqlClient;
using DemoBackend.Models;
using System;
using System.Collections.Generic;
using DemoBackend.Repository;
using Microsoft.Extensions.Configuration;
using DemoBackend.Dto;

namespace DemoBackend.Services.Autenticacion
{
    public class AutenticacionService : IAutenticacionService
    {

        private IConfiguration Configuration;
        private readonly IGenericRepositoryCTREntity<Login> _repository;
        private readonly IMapper _mapper;
        public AutenticacionService(IGenericRepositoryCTREntity<Login> repository, IMapper mapper, IConfiguration _configuration)
        {
            _repository = repository;
            _mapper = mapper;
            Configuration = _configuration;
        }

        public List<LoginDto> Login(string usuario, string contrasena)
        {
            string sigla = Configuration.GetSection("CredencialesConfig").GetSection("Sigla").Value;
            string idEmpresa = Configuration.GetSection("CredencialesConfig").GetSection("idEmpresa").Value;
            string sql = "ctr_credenciales_msctr @username,@password,@Sigla, @idEmpresa";
            var parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@username", usuario.Trim());
            parametros[1] = new SqlParameter("@password", contrasena);
            parametros[2] = new SqlParameter("@Sigla", sigla);
            parametros[3] = new SqlParameter("@idEmpresa", idEmpresa);
            return _mapper.Map<List<LoginDto>>(_repository.GetStoreProcedure(sql, parametros));
        }
    }
}
