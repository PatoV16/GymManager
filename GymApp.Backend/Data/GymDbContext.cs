using GymApp.Backend.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Backend.Data;

public class GymDbContext : DbContext
{
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
    {
    }

    // Cada DbSet representa una tabla en la BD
    public DbSet<Socio> Socios => Set<Socio>();
    public DbSet<Membresia> Membresias => Set<Membresia>();
    public DbSet<Pago> Pagos => Set<Pago>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== SOCIO =====
        modelBuilder.Entity<Socio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Cedula).IsUnique(); // Cédula única
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(150);
        });

        // ===== MEMBRESIA =====
        modelBuilder.Entity<Membresia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Tipo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Precio).HasColumnType("decimal(10,2)");

            entity.HasOne(e => e.Socio)
                  .WithMany(s => s.Membresias)
                  .HasForeignKey(e => e.SocioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== PAGO =====
        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Monto).HasColumnType("decimal(10,2)");
            entity.Property(e => e.MetodoPago).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Observaciones).HasMaxLength(500);

            entity.HasOne(e => e.Socio)
                  .WithMany(s => s.Pagos)
                  .HasForeignKey(e => e.SocioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
