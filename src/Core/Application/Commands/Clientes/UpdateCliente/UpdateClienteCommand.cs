using MediatR;

namespace connectasys_api.Core.Application.Commands.Clientes.UpdateCliente
{
    public class UpdateClienteCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
    }
}
