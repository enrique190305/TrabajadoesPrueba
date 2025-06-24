using Microsoft.EntityFrameworkCore;
using Prueba.Models;

namespace Prueba.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Departamento> Departamento { get; set; }
        public DbSet<Provincia> Provincia { get; set; }
        public DbSet<Distrito> Distrito { get; set; }
        public DbSet<Trabajador> Trabajadores { get; set; }
        
        // Agregamos el DbSet para el resultado del procedimiento almacenado

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuración de relaciones y propiedades
            modelBuilder.Entity<Provincia>()
                .HasOne<Departamento>()
                .WithMany()
                .HasForeignKey(p => p.IdDepartamento);

            modelBuilder.Entity<Distrito>()
                .HasOne<Provincia>()
                .WithMany()
                .HasForeignKey(d => d.IdProvincia);

            modelBuilder.Entity<Trabajador>()
                .HasOne<Departamento>()
                .WithMany()
                .HasForeignKey(t => t.IdDepartamento);

            modelBuilder.Entity<Trabajador>()
                .HasOne<Provincia>()
                .WithMany()
                .HasForeignKey(t => t.IdProvincia);

            modelBuilder.Entity<Trabajador>()
                .HasOne<Distrito>()
                .WithMany()
                .HasForeignKey(t => t.IdDistrito);


        }
    }
}