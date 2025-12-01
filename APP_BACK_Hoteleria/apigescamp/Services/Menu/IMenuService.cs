using DemoBackend.Dto.Menu;
using System.Collections.Generic;

namespace DemoBackend.Services.Menu
{
    public interface IMenuService
    {
        //cambio 1-12
        List<MenuDto> GetListaMenu(int IdUsuario, int IdPerfil);
        
    }
}
