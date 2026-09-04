using MediatR;

namespace connectasys_api.Core.Application.Commands.Clientes.UpdateCliente
{
    public enum UpdateClienteResult
    {
        Success,
        ClienteNotFound,
        DocumentoEmUso
    }

    public class UpdateClienteCommand : IRequest<UpdateClienteResult>
    {
        public int Id { get; set; }
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
    }
}
