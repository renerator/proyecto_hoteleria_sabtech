//using NotificadorJudicial.Utils.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotificadorJudicial.Helper
{
    public static class MenuHelper
    {
        /// <summary>
        /// Helper de Menú dinámico segun perfil de usuario, se carga respecto al tipo de usuario que inicia sesión
        /// sea Abogado o receptor, el menu se carga directo desde la BD, y cada controller en la aplicacion contiene SessionFilter
        /// para controlar el acceso.
        /// </summary>
        /// <param name="helper">No requiere parametros, el ID del usuario se envia por sesion.</param>
        /// <returns>Espacio de DIV de menú con nombre de Modulos y titulos de funcionalidades.</returns>
        //public static MvcHtmlString Menu(this HtmlHelper helper)
        //{
        //    //var menu = new Control();
        //    //var menulit = string.Empty;
        //    //if (menu.MenuUsuario(1).Count > 0)
        //    //{
        //    //    var urlMenu = menu.MenuUsuario(1).ToList();
        //    //    menulit = menulit + "<div id='sidebar-menu' class='main_menu_side hidden-print main_menu'><div class='menu_section'><h3>Menú Principal</h3><ul class='nav side-menu'>";
        //    //    for (var i = 0; i <= urlMenu.Count; i++)
        //    //    {
        //    //        if (i > urlMenu.Count)
        //    //        {
        //    //            break;
        //    //        }
        //    //        menulit = menulit + string.Format("<li><a><i class='{0}'></i>{1}<span class='fa fa-chevron-down'></span></a>", urlMenu[i].Clase, urlMenu[i].PieMenu);
        //    //        var nombreModulo = urlMenu[i].PieMenu;
        //    //        menulit = menulit + "<ul class='nav child_menu'>";
        //    //        for (var x = 0; x <= i; x++)
        //    //        {
        //    //            if (i >= urlMenu.Count)
        //    //            {
        //    //                break;
        //    //            }
        //    //            if (nombreModulo == urlMenu[i].PieMenu)
        //    //            {
        //    //                menulit = menulit + string.Format("<li><a href='../{2}/{1}'>{0}</a></li>", urlMenu[i].Titulo, urlMenu[i].Accion, urlMenu[i].Controller);
        //    //                i++;
        //    //            }
        //    //            else
        //    //            {
        //    //                i--;
        //    //                break;
        //    //            }
        //    //        }
        //    //        menulit = menulit + "</ul>";
        //    //        menulit = menulit + "</li>";
        //    //    }
        //    //    menulit = menulit + "</ul></div></div>";
        //    }
        //    return new MvcHtmlString(menulit);
        //}
    }
}