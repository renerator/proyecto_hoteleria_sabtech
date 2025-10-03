using DemoBackend.Dto.Menu;
using System.Collections.Generic;

namespace DemoBackend.Services.Menu
{
    public interface IMenuService
    {

        List<MenuDto> GetListaMenu(int IdUsuario, int IdPerfil);
        
    }
}
