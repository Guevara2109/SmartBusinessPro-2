using System.Configuration;
using Microsoft.EntityFrameworkCore;
using SmartBusinessPro.Models;

namespace SmartBusinessPro.Data;

/// <summary>
/// Contexto centralizado: único lugar donde se configura la conexión a SQL Server.
/// </summary>
public class SmartBusinessContext : DbContext
{
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        // Cadena de conexión leída desde App.config
        string? cadena = ConfigurationManager
            .ConnectionStrings["SmartBusinessPro"]?.ConnectionString;

        if (string.IsNullOrWhiteSpace(cadena))
            throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SmartBusinessPro' en App.config.");

        optionsBuilder.UseSqlServer(cadena);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>().ToTable("Categorias");
        modelBuilder.Entity<Producto>().ToTable("Productos");

        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoriaId);
    }
}
