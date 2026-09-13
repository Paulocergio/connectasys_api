using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Infrastructure.Persistence.Configurations
{
    public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
    {
        public void Configure(EntityTypeBuilder<Agendamento> builder)
        {
            builder.ToTable("agendamentos");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.EmpresaId).HasColumnName("empresa_id");
            builder.Property(a => a.TecnicoId).HasColumnName("tecnico_id");
            builder.Property(a => a.ClienteId).HasColumnName("cliente_id");
            builder.Property(a => a.VeiculoId).HasColumnName("veiculo_id");
            builder.Property(a => a.OrdemServicoId).HasColumnName("ordem_servico_id");
            builder.Property(a => a.DataHoraInicio).HasColumnName("data_hora_inicio");
            builder.Property(a => a.DataHoraFim).HasColumnName("data_hora_fim");
            builder.Property(a => a.Observacao).HasColumnName("observacao").HasColumnType("text");
            builder.Property(a => a.Status).HasColumnName("status").HasMaxLength(20);
            builder.Property(a => a.DataCadastro).HasColumnName("data_cadastro");

            builder.HasIndex(a => new { a.TecnicoId, a.DataHoraInicio });

            builder.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(a => a.TecnicoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(a => a.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<Veiculo>()
                .WithMany()
                .HasForeignKey(a => a.VeiculoId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<OrdemServico>()
                .WithMany()
                .HasForeignKey(a => a.OrdemServicoId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<Empresa>()
                .WithMany()
                .HasForeignKey(a => a.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
