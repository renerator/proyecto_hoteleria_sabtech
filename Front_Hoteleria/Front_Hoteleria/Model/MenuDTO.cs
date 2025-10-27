using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Model
{
    public class MenuDTO
    {
        public string Titulo { get; set; }
        public string Icono { get; set; } = "fa fa-folder";
        public string Url { get; set; }
        public List<MenuDTO> SubMenu { get; set; } = new List<MenuDTO>();
        public bool TieneHijos => SubMenu != null && SubMenu.Count > 0;
    }
}