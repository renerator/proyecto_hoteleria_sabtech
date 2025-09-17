using System.Threading.Tasks;
using DemoBackend.Models;

namespace DemoBackend.Services
{
    public interface IValidaUsuarioService
    {
        Task<bool> Validar();
    }
}