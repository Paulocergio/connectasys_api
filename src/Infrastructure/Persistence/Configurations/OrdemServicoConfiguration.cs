using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Infrastructure.Persistence.Configurations
{
    public class OrdemServicoConfiguration : IEntityTypeConfiguration<OrdemServico>
    {
        public void Configure(EntityTypeBuilder<OrdemServico> builder)
        {
            builder.ToTable("ordens_servico");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id).HasColumnName("id");
            builder.Property(o => o.ClienteId).HasColumnName("cliente_id");
            builder.Property(o => o.VeiculoId).HasColumnName("veiculo_id");
            builder.Property(o => o.TecnicoId).HasColumnName("tecnico_id");
            builder.Property(o => o.Status).HasColumnName("status").HasMaxLength(20);
            builder.Property(o => o.DescricaoProblema).HasColumnName("descricao_problema").HasColumnType("text");
            builder.Property(o => o.Diagnostico).HasColumnName("diagnostico").HasColumnType("text");
            builder.Property(o => o.Solucao).HasColumnName("solucao").HasColumnType("text");
            builder.Property(o => o.DataAbertura).HasColumnName("data_abertura");
            builder.Property(o => o.PrevisaoTermino).HasColumnName("previsao_termino");
            builder.Property(o => o.DataConclusao).HasColumnName("data_conclusao");
            builder.Property(o => o.ValorMaoDeObra).HasColumnName("valor_mao_de_obra").HasColumnType("decimal(10,2)");
            builder.Property(o => o.Desconto).HasColumnName("desconto").HasColumnType("decimal(10,2)");
            builder.Property(o => o.AprovacaoClienteEm).HasColumnName("aprovacao_cliente_em");
            builder.Property(o => o.AprovacaoClienteNome).HasColumnName("aprovacao_cliente_nome").HasMaxLength(150);

            builder.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(o => o.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Veiculo>()
                .WithMany()
                .HasForeignKey(o => o.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(o => o.TecnicoId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(o => o.Itens)
                .WithOne()
                .HasForeignKey(i => i.OrdemServicoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
