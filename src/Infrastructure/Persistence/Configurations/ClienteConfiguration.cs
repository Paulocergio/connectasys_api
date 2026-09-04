using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Infrastructure.Persistence.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("clientes");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.Nome).HasColumnName("nome").HasMaxLength(150);
            builder.Property(c => c.Email).HasColumnName("email").HasMaxLength(256);
            builder.Property(c => c.Telefone).HasColumnName("telefone").HasMaxLength(20);
            builder.Property(c => c.Cpf).HasColumnName("cpf").HasMaxLength(11);
            builder.Property(c => c.Cnpj).HasColumnName("cnpj").HasMaxLength(14);
            builder.Property(c => c.RazaoSocial).HasColumnName("razao_social").HasMaxLength(200);
            builder.Property(c => c.Cep).HasColumnName("cep").HasMaxLength(8);
            builder.Property(c => c.Logradouro).HasColumnName("logradouro").HasMaxLength(200);
            builder.Property(c => c.Bairro).HasColumnName("bairro").HasMaxLength(100);
            builder.Property(c => c.Municipio).HasColumnName("municipio").HasMaxLength(100);
            builder.Property(c => c.Uf).HasColumnName("uf").HasMaxLength(2);
            builder.Property(c => c.DataCadastro).HasColumnName("data_cadastro");
        }
    }
}
