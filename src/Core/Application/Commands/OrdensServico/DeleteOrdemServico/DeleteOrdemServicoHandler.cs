using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.OrdensServico.DeleteOrdemServico
{
    public class DeleteOrdemServicoHandler : IRequestHandler<DeleteOrdemServicoCommand, bool>
    {
        private readonly IOrdemServicoRepository _repository;
        private readonly IEstoqueRepository _estoqueRepository;

        public DeleteOrdemServicoHandler(IOrdemServicoRepository repository, IEstoqueRepository estoqueRepository)
        {
            _repository = repository;
            _estoqueRepository = estoqueRepository;
        }

        public async Task<bool> Handle(DeleteOrdemServicoCommand request, CancellationToken cancellationToken)
        {
            var ordemServico = await _repository.GetByIdAsync(request.Id);
            if (ordemServico is null) return false;

            foreach (var item in ordemServico.Itens.Where(i => i.EstoqueId is not null))
            {
                var estoque = await _estoqueRepository.GetByIdAsync(item.EstoqueId!.Value);
                if (estoque is not null)
                {
                    estoque.Quantidade += item.Quantidade;
                    await _estoqueRepository.UpdateAsync(estoque);
                }
            }

            await _repository.DeleteAsync(ordemServico);
            return true;
        }
    }
}
