using Microsoft.EntityFrameworkCore;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Infrastructure.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Veiculo> Veiculos => Set<Veiculo>();
        public DbSet<ContaPagar> ContasPagar => Set<ContaPagar>();
        public DbSet<ContaReceber> ContasReceber => Set<ContaReceber>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
