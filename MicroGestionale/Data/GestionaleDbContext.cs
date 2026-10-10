using Microsoft.EntityFrameworkCore;
using MicroGestionale.Models;

namespace MicroGestionale.Data;

public class GestionaleDbContext : DbContext
{
    public GestionaleDbContext(
        DbContextOptions<GestionaleDbContext> options)
    : base(options)
    {
    }

    public DbSet<Cliente> Clienti { get; set; }

    public DbSet<Preventivo> Preventivi { get; set; }

    public DbSet<RigaPreventivo> RighePreventivo { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Il numero del preventivo deve essere univoco
        modelBuilder.Entity<Preventivo>()
            .HasIndex(p => p.Numero)
            .IsUnique();

        // Un cliente con preventivi non può essere eliminato:
        // va bloccato, così lo storico resta intatto
        modelBuilder.Entity<Preventivo>()
            .HasOne(p => p.Cliente)
            .WithMany(c => c.Preventivi)
            .HasForeignKey(p => p.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
