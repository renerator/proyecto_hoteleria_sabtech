
using DemoBackend.Models.Mantenedores;
using DemoBackend.Models.Habitacion;
using DemoBackend.Models.Reserva;
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




    }
}