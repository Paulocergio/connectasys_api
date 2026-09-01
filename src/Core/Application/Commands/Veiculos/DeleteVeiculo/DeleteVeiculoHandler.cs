using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.Veiculos.DeleteVeiculo
{
    public class DeleteVeiculoHandler : IRequestHandler<DeleteVeiculoCommand, bool>
    {
        private readonly IVeiculoRepository _repository;

        public DeleteVeiculoHandler(IVeiculoRepository repository) => _repository = repository;

        public async Task<bool> Handle(DeleteVeiculoCommand request, CancellationToken cancellationToken)
        {
            var veiculo = await _repository.GetByIdAsync(request.Id);
            if (veiculo is null) return false;

            await _repository.DeleteAsync(veiculo);
            return true;
        }
    }
}
