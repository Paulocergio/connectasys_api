using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Infrastructure.Persistence.Configurations
{
    public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.ToTable("empresas");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(150);
            builder.Property(e => e.DataCadastro).HasColumnName("data_cadastro");
            builder.Property(e => e.TrialExpiraEm).HasColumnName("trial_expira_em");
        }
    }
}
