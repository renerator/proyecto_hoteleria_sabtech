using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using DemoBackend.Configurations;
using DemoBackend.Models;
using DemoBackend.Repositories;


namespace DemoBackend.Services
{
    public class ValidaUsuarioService : IValidaUsuarioService
    {
        private readonly CredencialesConfig credencialesConfig;
        private readonly IGenericRepositoryCTR<CtrCredencial> credencialRepository;

        public ValidaUsuarioService(IOptions<CredencialesConfig> credencialesConfig, IGenericRepositoryCTR<CtrCredencial> credencialRepository)
        {
            this.credencialesConfig = credencialesConfig.Value;
            this.credencialRepository = credencialRepository;
        }

        public async Task<bool> Validar()
        {
            var usuarios = await credencialRepository.WhereAsync(x => x.Username == credencialesConfig.Username && x.Password == credencialesConfig.Password && x.Conexion == credencialesConfig.Conexion);

            return usuarios != null && usuarios.Count() > 0;
        }
    }
}