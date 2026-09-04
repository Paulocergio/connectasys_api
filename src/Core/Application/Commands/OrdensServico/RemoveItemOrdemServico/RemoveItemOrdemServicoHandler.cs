using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.OrdensServico.RemoveItemOrdemServico
{
    public class RemoveItemOrdemServicoHandler : IRequestHandler<RemoveItemOrdemServicoCommand, bool>
    {
        private readonly IOrdemServicoRepository _repository;

        public RemoveItemOrdemServicoHandler(IOrdemServicoRepository repository) => _repository = repository;

        public async Task<bool> Handle(RemoveItemOrdemServicoCommand request, CancellationToken cancellationToken)
        {
            var item = await _repository.GetItemByIdAsync(request.ItemId);
            if (item is null) return false;

            await _repository.RemoveItemAsync(item);
            return true;
        }
    }
}
