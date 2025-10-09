using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using DemoBackend.Services.Autenticacion;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DemoBackend.Dto;

namespace DemoBackend.Controllers
{
    public class AutenticacionController : BaseController
    {
        private readonly IAutenticacionService _service;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public AutenticacionController(
            IAutenticacionService service,
            IConfiguration config,
            ILogger<AutenticacionController> logger)
        {
            _config = config;
            _service = service;
            _logger = logger;
        }

        /// <summary>Login de usuarios</summary>
        [AllowAnonymous]
        [HttpPost("Login")]
#pragma warning disable CS1998
        public async Task<IActionResult> Login(LoginDto user)
#pragma warning restore CS1998
        {
            if (string.IsNullOrWhiteSpace(user?.username) || string.IsNullOrWhiteSpace(user?.password))
            {
                _logger.LogInformation("Login: Datos de autenticación inválidos");
                return StatusCode(400, "Login: Datos de autenticación inválidos");
            }

            try
            {
                var login = _service.Login(user.username, user.password);
                if (login == null || login.Count == 0)
                {
                    _logger.LogWarning("Login: Usuario o contraseña no son correctos");
                    return StatusCode(400, "Usuario o contraseña no son correctos");
                }

                var first = login.First();
                int idUser = first.idUsuario;
                int idPerfil = first.idPerfil;

                // Para auditoría: el filtro leerá esto SOLO en /Login
                HttpContext.Items["idUsuarioLogin"] = idUser;

                var tokenString = GenerateJSONWebToken(idUser, idPerfil, user.username);
                _logger.LogInformation($"Login: Usuario {user.username} válido");

                return Ok(new { Token = tokenString, Message = "Success", IdUser = idUser, IdPerfil = idPerfil });
            }
            catch (Exception e)
            {
                _logger.LogError($"Login: Ha ocurrido un error --> {e.Message}");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }
        }

        #region GenerateJWT
        /// <summary>Genera JWT con claims (idUsuario, idPerfil)</summary>
        private string GenerateJSONWebToken(int idUsuario, int idPerfil, string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                // estándar
                new Claim(JwtRegisteredClaimNames.Sub, username ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // donde ya lo tenías (id como UniqueName)
                new Claim(JwtRegisteredClaimNames.UniqueName, idUsuario.ToString()),
                // explícitos (recomendado para la API/filtros)
                new Claim("idUsuario", idUsuario.ToString()),
                new Claim("idPerfil", idPerfil.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],      // si no usas Audience, puedes igualarlo al Issuer
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),   // evita 100 años :)
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
    }
}

