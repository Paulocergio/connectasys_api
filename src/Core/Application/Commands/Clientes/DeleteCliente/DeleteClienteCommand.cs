using MediatR;

namespace connectasys_api.Core.Application.Commands.Clientes.DeleteCliente
{
    public class DeleteClienteCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
