using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Infrastructure.Persistence.Context
{
    // Garante Kind=Utc em toda propriedade DateTime/DateTime? antes de gravar no
    // Postgres — colunas timestamptz rejeitam DateTimeKind.Unspecified (ex.: datas
    // vindas de <input type="date"> no front, que chegam sem fuso no JSON).
    public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter() : base(
            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        {
        }
    }

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Veiculo> Veiculos => Set<Veiculo>();
        public DbSet<ContaPagar> ContasPagar => Set<ContaPagar>();
        public DbSet<ContaReceber> ContasReceber => Set<ContaReceber>();
        public DbSet<OrdemServico> OrdensServico => Set<OrdemServico>();
        public DbSet<ItemOrdemServico> ItensOrdemServico => Set<ItemOrdemServico>();
        public DbSet<Estoque> Estoque => Set<Estoque>();
        public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
        public DbSet<Empresa> Empresas => Set<Empresa>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        }
    }
}
