using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using DemoBackend.Services.Autenticacion;
using DemoBackend.Models;
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
        public AutenticacionController(IAutenticacionService service, IConfiguration config ,ILogger<AutenticacionController> logger)
        {
            _config = config;
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Login de usuarios
        /// </summary>
        /// <param name="user">usuario y pass</param>
        /// <returns>token</returns>
        /// <response code="200">token</response>
        /// <response code="204">No encuentra datos</response>
        /// <response code="400">Datos entradas inválidos</response>
        /// <response code="500">Error interno</response>
        [AllowAnonymous]
        [HttpPost("Login")]
#pragma warning disable CS1998 // El método asincrónico carece de operadores "await" y se ejecutará de forma sincrónica
        public async Task<IActionResult> Login(LoginDto user)
#pragma warning restore CS1998 // El método asincrónico carece de operadores "await" y se ejecutará de forma sincrónica
        {

            if (user.username.Length <= 0 || user.password.Length <= 0)
            {
                _logger.LogInformation($"Login: Datos de autenticación invalidos  ");
                return StatusCode(400, "Login: Datos de autenticación invalidos");
            }

            try
            {
                var login = _service.Login(user.username, user.password);

                if (login.Count == 0)
                {
                    _logger.LogWarning($"Login: Usuario o contraseña no son correctos");
                    return StatusCode(400, $"Login: Usuario o contraseña no son correctos");
                }

                else
                {
                    var tokenString = GenerateJSONWebToken(login.FirstOrDefault());
                    _logger.LogInformation($"Login: Usuario { user.username } válido");
                    return Ok(new { Token = tokenString, Message = "Success" });
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Login: Ha ocurrido el siguiente error en la autenticación de usuarios --> { e.Message }");
                _logger.LogTrace(e.StackTrace);
                return StatusCode(500, e.Message);
            }

        }

        #region GenerateJWT
        /// <summary>
        /// Generate Json Web Token Method
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private string GenerateJSONWebToken(LoginDto userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddYears(100),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion


    }
}
