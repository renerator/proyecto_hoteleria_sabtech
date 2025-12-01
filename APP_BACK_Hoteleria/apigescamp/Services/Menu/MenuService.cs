using AutoMapper;
using DemoBackend.Dto.Mantenedores;
using DemoBackend.Dto.Menu;
using DemoBackend.Models.Menu;
using DemoBackend.RepositoryGes;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DemoBackend.Services.Menu
{
    public class MenuService : IMenuService
    {
        private readonly IGenericRepositoryEntity<MenuModels> _listamenu;
        private readonly IMapper _mapper;
        //cambio 1-12
        public MenuService(
            IGenericRepositoryEntity<MenuModels> listamenu,
            IMapper mapper)
        {
            _listamenu = listamenu;
            _mapper = mapper;
        }

        #region Menu
        public List<MenuDto> GetListaMenu(int IdUsuario, int IdPerfil)
        {
            string sql = "CTR_LISTADO_MENU @idusuario, @idperfil";

            var parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@idusuario", IdUsuario);
            parametros[1] = new SqlParameter("@idperfil", IdPerfil);

            var lista = _listamenu.GetStoreProcedure(sql, parametros);

            // No se necesita mapear porque ya usamos DTO directamente
            return _mapper.Map<List<MenuDto>>(lista);
        }
        #endregion
    }
}



