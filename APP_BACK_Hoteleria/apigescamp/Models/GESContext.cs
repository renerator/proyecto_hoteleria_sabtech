
using DemoBackend.Dto.Habitacion;
using DemoBackend.Models.Bodega;
using DemoBackend.Models.Calendario;
using DemoBackend.Models.Campamentos;
using DemoBackend.Models.Contratos;
using DemoBackend.Models.Dotaciones;
using DemoBackend.Models.EmpresaContratista;
using DemoBackend.Models.Check;
using DemoBackend.Models.Huesped;
using DemoBackend.Models.EstadoReserva;
using DemoBackend.Models.Habitacion;
using DemoBackend.Models.HabitacionInsumo;
using DemoBackend.Models.Insumos;
using DemoBackend.Models.Inventario;
using DemoBackend.Models.Mantenedores;
using DemoBackend.Models.Menu;
using DemoBackend.Models.OrdenTrabajo;
using DemoBackend.Models.Reserva;
using DemoBackend.Models.Servicio;
using DemoBackend.Models.ServicioCategoria;
using DemoBackend.Models.ServicioEstado;
using DemoBackend.Models.ServicioPrioridad;
using DemoBackend.Models.ServiciosPersonal;
using DemoBackend.Models.SolicitudServicio;
using DemoBackend.Models.TipoHabitacion;
using DemoBackend.Models.Trabajador;
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

        public virtual DbSet<ServicioHuespedModels> ServiciosHuesped { get; set; }

        public virtual DbSet<AreasModels> ListaAreas { get; set; }
        public virtual DbSet<HabitacionModels> ListaHabitaciones { get; set; }
        public virtual DbSet<ReservaModels> ListaReservas { get; set; }
        public virtual DbSet<HabitacionInsumoModels> ListaHabitacionInsumo { get; set; }
        public virtual DbSet<ReservaDashboardKPI> Dashboard { get; set; }

        public virtual DbSet<ReservaHuespedModels> ReservaHuesped { get; set; }
        public DbSet<EmpresaContratistaModels> EmpresasContratistas { get; set; }

        public virtual DbSet<ReservaDashboardPanelPrincipalModel> DashboardPanelPrincipal { get; set; }
        public virtual DbSet<ReservaDashboardKPI> ReservaTrabajador { get; set; }
        public virtual DbSet<HabitacionDashboardModels> DashboardHabitacion { get; set; }

        public virtual DbSet<SolicitudServicioModels> SolicitudServicio { get; set; }
    
        public virtual DbSet<ServicioPrioridadModels> ServicioPrioridadServicio { get; set; }
        public virtual DbSet<ServicioEstadoModels> ServicioEstadoServicio { get; set; }
        public virtual DbSet<ServicioKpi> ServicioKPIServicio { get; set; }


        public virtual DbSet<ServicioCategoriaModels> ServicioCategoriaServicio { get; set; }
        public virtual DbSet<TipoHabitacionModels> TipoHabiatacion { get; set; }
        public virtual DbSet<EstadoReservaModels> EstadoReserva { get; set; }
        public virtual DbSet<OrdenTrabajoModels> ListaOrdenTrabajo { get; set; }
        public virtual DbSet<SolicitudKPIModels> SolicitudKPI{ get; set; }

        public DbSet<CalendarioEventosModels> CalendarioEventos { get; set; }
        public DbSet<CalendarioBloqueosModels> CalendarioBloqueos { get; set; }
        public DbSet<CalendarioMantenimientosModels> CalendarioMantenimientos { get; set; }
        public DbSet<CalendarioSanitizacionModels> CalendarioSanitizacion { get; set; }


        public DbSet<HuespedReclamoModels> ReclamosHuesped { get; set; }

        public DbSet<CampamentosModels> Campamentos { get; set; }

        public DbSet<CheckModels> Check { get; set; }
        public DbSet<CheckKPIModels> CheckKPI { get; set; }
        public DbSet<CampamentoAreasModels> CampamentoAreas { get; set; }

        public DbSet<ContratosModels> Contratos { get; set; }
        public DbSet<ContratoTrabajadoresModels> ContratoTrabajadores { get; set; }

        public DbSet<DotacionesModels> Dotaciones { get; set; }

        public DbSet<InventarioModels> Inventario { get; set; }
        public DbSet<InventarioMovimientosModels> InventarioMovimientos { get; set; }

        public DbSet<ServiciosPersonalModels> ServiciosPersonal { get; set; }
        public DbSet<CampamentoKPIModels> CampamentoKPI { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReservaDashboardKPI>(eb =>
            {
                eb.HasNoKey();     // <- clave
                eb.ToView(null);   // <- no está mapeado a vista/tabla
            });

            modelBuilder.Entity<CheckKPIModels>(eb =>
            {
                eb.HasNoKey();     // <- clave
                eb.ToView(null);   // <- no está mapeado a vista/tabla
            });

            modelBuilder.Entity<CampamentoKPIModels>(eb =>
            {
                eb.HasNoKey();     // <- clave
                eb.ToView(null);   // <- no está mapeado a vista/tabla
            });

            modelBuilder.Entity<ServicioKpi>(eb =>
            {
                eb.HasNoKey();     // <- clave
                eb.ToView(null);   // <- no está mapeado a vista/tabla
            });
            modelBuilder.Entity<TipoHabitacionModels>(eb =>
            {
                eb.HasNoKey();     // <- clave
                eb.ToView(null);   // <- no está mapeado a vista/tabla
            });

           

            modelBuilder.Entity<ReservaDashboardPanelPrincipalModel>(eb =>
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