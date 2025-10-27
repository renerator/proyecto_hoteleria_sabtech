using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Front_Hoteleria.Model.Huesped.Servicio
{
    public class SolicitudServicioVm
    {
        [Display(Name = "Tipo de Servicio"), Required]
        public int? TipoServicioId { get; set; }

        [Display(Name = "Prioridad"), Required]
        public int? PrioridadId { get; set; }

        [Display(Name = "Descripción del Servicio"), Required, StringLength(1000)]
        public string Descripcion { get; set; }

        [Display(Name = "Hora Preferida")]
        public string HoraPreferida { get; set; } // HH:mm

        [Display(Name = "Método de Contacto"), Required]
        public int? MetodoContactoId { get; set; }

        [Display(Name = "Notas Adicionales")]
        public string NotasAdicionales { get; set; }

        // Dropdowns
        public IEnumerable<SelectListItem> Tipos { get; set; }
        public IEnumerable<SelectListItem> Prioridades { get; set; }
        public IEnumerable<SelectListItem> Metodos { get; set; }

        // Estadísticas (panel derecho)
        public int TotalSolicitudes { get; set; }
        public int Completadas { get; set; }
        public int Pendientes { get; set; }
    }
}
