using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.OrdensServico.DeleteOrdemServico
{
    public class DeleteOrdemServicoHandler : IRequestHandler<DeleteOrdemServicoCommand, bool>
    {
        private readonly IOrdemServicoRepository _repository;

        public DeleteOrdemServicoHandler(IOrdemServicoRepository repository) => _repository = repository;

        public async Task<bool> Handle(DeleteOrdemServicoCommand request, CancellationToken cancellationToken)
        {
            var ordemServico = await _repository.GetByIdAsync(request.Id);
            if (ordemServico is null) return false;

            await _repository.DeleteAsync(ordemServico);
            return true;
        }
    }
}
