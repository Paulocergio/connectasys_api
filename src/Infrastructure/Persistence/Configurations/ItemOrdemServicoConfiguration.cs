using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Infrastructure.Persistence.Configurations
{
    public class ItemOrdemServicoConfiguration : IEntityTypeConfiguration<ItemOrdemServico>
    {
        public void Configure(EntityTypeBuilder<ItemOrdemServico> builder)
        {
            builder.ToTable("itens_ordem_servico");
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id).HasColumnName("id");
            builder.Property(i => i.OrdemServicoId).HasColumnName("ordem_servico_id");
            builder.Property(i => i.Descricao).HasColumnName("descricao").HasMaxLength(200);
            builder.Property(i => i.Quantidade).HasColumnName("quantidade").HasColumnType("decimal(10,2)");
            builder.Property(i => i.ValorUnitario).HasColumnName("valor_unitario").HasColumnType("decimal(10,2)");
        }
    }
}
