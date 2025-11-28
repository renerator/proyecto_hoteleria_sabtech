using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Front_Hoteleria.Dto.OrdenTrabajo
{
    public class OrdenTrabajoDto
    {
        public int IdOrdenTrabajo { get; set; }

        public string NumeroOT { get; set; }

        public DateTime FechaIngresoOT { get; set; }

        public DateTime? FechaCierreOT { get; set; }

        public int IdHabitacion { get; set; }

        public string Estado { get; set; }

        public int? IdTipo { get; set; }

        public string NombreTipo { get; set; }

        public string Descripcion { get; set; }

        public string Prioridad { get; set; }

        public int? IdPrioridad { get; set; }

        public int? IdEstado { get; set; }

        public int? IdTecnico { get; set; }

        public int? TiempoMinutos { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? IdUsuario { get; set; }
        public string Tecnico { get; set; }

        
        public int? IdTipoTrabajo { get; set; }
        
        public int? IdTecnicoPreferido { get; set; }
        public int? IdContactoResponsable { get; set; }

        public DateTime? FechaProgramada { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? HoraProgramada { get; set; }   // o TimeSpan? según tu diseño
        public string TituloTrabajo { get; set; }
        public string DescripcionDetallada { get; set; }
        public string MaterialesNecesarios { get; set; }
        public string CostoEstimado { get; set; }
        public decimal? TiempoEstimadoHoras { get; set; }
        public string ObservacionesAdicionales { get; set; }

        // Combos:
        public IEnumerable<SelectListItem> Habitaciones { get; set; }
        public IEnumerable<SelectListItem> TiposTrabajo { get; set; }
        public IEnumerable<SelectListItem> Prioridades { get; set; }
        public IEnumerable<SelectListItem> Tecnicos { get; set; }
        public IEnumerable<SelectListItem> Contactos { get; set; }
    }
}
