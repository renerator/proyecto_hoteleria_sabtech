using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace DemoBackend.Models
{
    public partial class CTRContext : DbContext
    {
        public CTRContext()
        {
        }

        public CTRContext(DbContextOptions<CTRContext> options)
            : base(options)
        {
            
        }

        public virtual DbSet<Login> Login { get; set; }
        public virtual DbSet<CtrCredencial> CtrCredencials { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<CtrCredencial>(entity =>
            {
                entity.HasKey(e => e.IdCredencial)
                    .HasName("PK_crt_credencial");

                entity.ToTable("ctr_credencial");

                entity.Property(e => e.IdCredencial).HasColumnName("idCredencial");

                entity.Property(e => e.Conexion)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoRegistro)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("estadoRegistro");

                entity.Property(e => e.FechaActualizacion)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaActualizacion");

                entity.Property(e => e.IdTipo).HasColumnName("idTipo");

                entity.Property(e => e.IdUsuarioActualizacion).HasColumnName("idUsuarioActualizacion");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Provider)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sigla)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });            
        }
    }
}
