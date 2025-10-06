
using DemoBackend.Models.Mantenedores;
using DemoBackend.Models.Habitacion;
using DemoBackend.Models.Reserva;
using DemoBackend.Models.Trabajador;
using DemoBackend.Models.Servicio;
using DemoBackend.Models.Menu;
using Microsoft.EntityFrameworkCore;

namespace DemoBackend.Models
{
    public class GESContext : DbContext
    {
        public GESContext()
        {
        }

        public GESContext(DbContextOptions<GESContext> options)
            : base(options)
        {
        }


        public virtual DbSet<AreasModels> ListaAreas { get; set; }
        public virtual DbSet<HabitacionModels> ListaHabitaciones { get; set; }
        public virtual DbSet<ReservaModels> ListaReservas { get; set; }
        public virtual DbSet<MenuModels> ListaMenus { get; set; }

        public virtual DbSet<TrabajadorModels> ListaTrabajadores { get; set; }
        public virtual DbSet<ServicioModels> ListaServicio { get; set; }




    }
}