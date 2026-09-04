using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Infrastructure.Persistence.Configurations
{
    public class ContaPagarConfiguration : IEntityTypeConfiguration<ContaPagar>
    {
        public void Configure(EntityTypeBuilder<ContaPagar> builder)
        {
            builder.ToTable("contas_pagar");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.Descricao).HasColumnName("descricao").HasMaxLength(200);
            builder.Property(c => c.Fornecedor).HasColumnName("fornecedor").HasMaxLength(150);
            builder.Property(c => c.Valor).HasColumnName("valor").HasColumnType("numeric(12,2)");
            builder.Property(c => c.DataVencimento).HasColumnName("data_vencimento");
            builder.Property(c => c.DataPagamento).HasColumnName("data_pagamento");
            builder.Property(c => c.FormaPagamento).HasColumnName("forma_pagamento").HasMaxLength(20);
            builder.Property(c => c.DataCadastro).HasColumnName("data_cadastro");
        }
    }
}
