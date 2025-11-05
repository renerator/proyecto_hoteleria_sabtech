using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Dto.ServiciosDisponibles
{
    public class ServicioDisponibleDto
    {

        public int IdServicio { get; set; }             // PK
        public string NombreServicio { get; set; }      // Ej: Limpieza, Reparación
        public int IdTipoServicio { get; set; }         // FK tipo
        public int IdEmpresa { get; set; }              // FK empresa
        public bool Estado { get; set; }                // Activo/Inactivo

        // Nuevos campos
        public int? IdServicioEstado { get; set; }
        public int? IdServicioPrioridad { get; set; }   // FK a ctr_man_ServicioPrioridad
        public int? IdServiciosCategoria { get; set; }  // FK a ctr_man_ServiciosCategoria
        public int TiempoEstimadoMinutos { get; set; }  // minutos (mapea a columna TiempoEsttimado)
        public int? Precio { get; set; }             // DECIMAL(19,4) en BD

        // nuevos que vienen del SP
        public string NombreEstadoServicio { get; set; }
        public string NombreCategoria { get; set; }
        public string NombrePrioridad { get; set; }

        // (Opcionales para la UI; si no los usarás, elimínalos)
        public string TiempoEstimadoFmt =>
            TimeSpan.FromMinutes(TiempoEstimadoMinutos).ToString(@"hh\:mm");
    }
}