using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Models
{
    public class MenuItem
    {
        public string Titulo { get; set; }
        public string Icono { get; set; } = "fa fa-folder";
        public string Url { get; set; }
        public List<MenuItem> SubMenu { get; set; } = new List<MenuItem>();
        public bool TieneHijos => SubMenu != null && SubMenu.Count > 0;
    }
}