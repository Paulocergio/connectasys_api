using connectasys_api.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace connectasys_api.Infrastructure.Persistence.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure( EntityTypeBuilder<Usuario> builder) 

        {

            builder.ToTable("usuarios");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).HasColumnName("id");
            builder.Property(u => u.Nome).HasColumnName("nome").HasMaxLength(150);
            builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(256);
            builder.Property(u => u.Role).HasColumnName("role").HasMaxLength(20);
            builder.Property(u => u.Telefone).HasColumnName("telefone").HasMaxLength(20);
            builder.Property(u => u.SenhaHash).HasColumnName("senha_hash").HasMaxLength(60);
            builder.Property(u => u.DataCriacaoUtc).HasColumnName("data_criacao_utc");

            builder.HasIndex(u => u.Email).IsUnique();

        }
    }
}
