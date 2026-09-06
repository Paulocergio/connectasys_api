using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Infrastructure.Persistence.Configurations
{
    public class ContaReceberConfiguration : IEntityTypeConfiguration<ContaReceber>
    {
        public void Configure(EntityTypeBuilder<ContaReceber> builder)
        {
            builder.ToTable("contas_receber");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.ClienteId).HasColumnName("cliente_id");
            builder.Property(c => c.Descricao).HasColumnName("descricao").HasMaxLength(200);
            builder.Property(c => c.Valor).HasColumnName("valor").HasColumnType("numeric(12,2)");
            builder.Property(c => c.DataVencimento).HasColumnName("data_vencimento");
            builder.Property(c => c.DataRecebimento).HasColumnName("data_recebimento");
            builder.Property(c => c.FormaPagamento).HasColumnName("forma_pagamento").HasMaxLength(20);
            builder.Property(c => c.DataCadastro).HasColumnName("data_cadastro");
            builder.Property(c => c.OrdemServicoId).HasColumnName("ordem_servico_id");

            builder.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<OrdemServico>()
                .WithMany()
                .HasForeignKey(c => c.OrdemServicoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
