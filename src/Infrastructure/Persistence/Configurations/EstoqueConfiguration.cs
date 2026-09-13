using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Infrastructure.Persistence.Configurations
{
    public class EstoqueConfiguration : IEntityTypeConfiguration<Estoque>
    {
        public void Configure(EntityTypeBuilder<Estoque> builder)
        {
            builder.ToTable("estoque");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.EmpresaId).HasColumnName("empresa_id");
            builder.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(200);
            builder.Property(e => e.Descricao).HasColumnName("descricao").HasMaxLength(500);
            builder.Property(e => e.Quantidade).HasColumnName("quantidade").HasColumnType("numeric(10,2)");
            builder.Property(e => e.PrecoCompra).HasColumnName("preco_compra").HasColumnType("numeric(10,2)");
            builder.Property(e => e.PrecoVenda).HasColumnName("preco_venda").HasColumnType("numeric(10,2)");
            builder.Property(e => e.DataCadastro).HasColumnName("data_cadastro");

            builder.HasOne<Empresa>()
                .WithMany()
                .HasForeignKey(e => e.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
