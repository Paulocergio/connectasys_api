using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.Clientes.DeleteCliente
{
    public class DeleteClienteHandler : IRequestHandler<DeleteClienteCommand, bool>
    {
        private readonly IClienteRepository _repository;

        public DeleteClienteHandler(IClienteRepository repository) => _repository = repository;

        public async Task<bool> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _repository.GetByIdAsync(request.Id);
            if (cliente is null) return false;

            await _repository.DeleteAsync(cliente);
            return true;
        }
    }
}
