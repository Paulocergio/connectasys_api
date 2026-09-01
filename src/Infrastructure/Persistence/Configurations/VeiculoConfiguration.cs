using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Infrastructure.Persistence.Configurations
{
    public class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
    {
        public void Configure(EntityTypeBuilder<Veiculo> builder)
        {
            builder.ToTable("veiculos");
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id).HasColumnName("id");
            builder.Property(v => v.ClienteId).HasColumnName("cliente_id");
            builder.Property(v => v.Placa).HasColumnName("placa").HasMaxLength(10);
            builder.Property(v => v.Marca).HasColumnName("marca").HasMaxLength(50);
            builder.Property(v => v.Modelo).HasColumnName("modelo").HasMaxLength(50);
            builder.Property(v => v.Ano).HasColumnName("ano");
            builder.Property(v => v.Cor).HasColumnName("cor").HasMaxLength(30);
            builder.Property(v => v.DataCadastro).HasColumnName("data_cadastro");

            builder.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
