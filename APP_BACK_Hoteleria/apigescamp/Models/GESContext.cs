
using DemoBackend.Dto.Habitacion;
using DemoBackend.Models.Bodega;
using DemoBackend.Models.Habitacion;
using DemoBackend.Models.HabitacionInsumo;
using DemoBackend.Models.Insumos;
using DemoBackend.Models.Mantenedores;
using DemoBackend.Models.Menu;
using DemoBackend.Models.Reserva;
using DemoBackend.Models.Servicio;
using DemoBackend.Models.Trabajador;
using DemoBackend.Models.SolicitudServicio;
using DemoBackend.Models.OrdenTrabajo;
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
        public virtual DbSet<HabitacionInsumoModels> ListaHabitacionInsumo { get; set; }
        public virtual DbSet<ReservaDashboardKPI> Dashboard { get; set; }
        public virtual DbSet<ReservaDashboardKPI> ReservaTrabajador { get; set; }
        public virtual DbSet<HabitacionDashboardModels> DashboardHabitacion { get; set; }

        public virtual DbSet<SolicitudServicioModels> SolicitudServicio { get; set; }
        public virtual DbSet<OrdenTrabajoModels> ListaOrdenTrabajo { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReservaDashboardKPI>(eb =>
            {
                eb.HasNoKey();     // <- clave
                eb.ToView(null);   // <- no está mapeado a vista/tabla
            });
            modelBuilder.Entity<HabitacionDashboardModels>(eb =>
            {
                eb.HasNoKey();     // <- clave
                eb.ToView(null);   // <- no está mapeado a vista/tabla
            });
            modelBuilder.Entity<ReservaTrabajadorModels>(eb =>
            {
                eb.HasNoKey();     // <- clave
                eb.ToView(null);   // <- no está mapeado a vista/tabla
            });
           
            // ...tus otras entidades
        }

        

      
        public virtual DbSet<MenuModels> ListaMenus { get; set; }

        public virtual DbSet<TrabajadorModels> ListaTrabajadores { get; set; }
       
        public virtual DbSet<InsumoModels> ListaInsumos { get; set; }
        public virtual DbSet<BodegaModels> ListaBodegas { get; set; }
        public virtual DbSet<ServicioModels> ListaServicio { get; set; }




    }
}