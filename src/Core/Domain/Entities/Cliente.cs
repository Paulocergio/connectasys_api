namespace connectasys_api.Core.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public Guid EmpresaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string? Cpf { get; set; }
        public string? Cnpj { get; set; }
        public string? RazaoSocial { get; set; }
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Bairro { get; set; }
        public string? Municipio { get; set; }
        public string? Uf { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}
