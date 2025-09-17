
using DemoBackend.Models.Mantenedores;
using Microsoft.EntityFrameworkCore;

namespace DemoBackend.Models
{
    public class GESContext: DbContext
    {
        public GESContext()
        {
        }

        public GESContext(DbContextOptions<GESContext> options)
            : base(options)
        {
        }
       
       
        public virtual DbSet<AreasModels> ListaAreas { get; set; }

       


    }
}