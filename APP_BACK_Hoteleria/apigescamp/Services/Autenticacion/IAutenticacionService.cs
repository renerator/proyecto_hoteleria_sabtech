using DemoBackend.Models;
using System.Collections.Generic;
using DemoBackend.Dto;

namespace DemoBackend.Services.Autenticacion
{
    public interface IAutenticacionService
    {
        List<LoginDto> Login(string usuario, string contrasena);
    }
}
