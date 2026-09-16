using Microsoft.EntityFrameworkCore;
using MicroGestionale.Models;
using System.Collections.Generic;

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
}