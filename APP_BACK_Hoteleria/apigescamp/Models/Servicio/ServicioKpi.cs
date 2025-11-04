using DemoBackend.Models;
using DemoBackend.Models.ServicioCategoria;
using DemoBackend.Models.ServicioEstado;
using DemoBackend.Models.ServicioPrioridad;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DemoBackend.Models.Servicio
{


    
    public class ServicioKpi : EntityBase   {
        

        public int? TotalServicios { get; set; }       
        public int? ServiciosActivos { get; set; }     
        public int? Categorias { get; set; }              
        public int? PromedioMinutos { get; set; }
    }
}




