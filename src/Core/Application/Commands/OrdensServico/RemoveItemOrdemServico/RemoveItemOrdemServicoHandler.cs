using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.OrdensServico.RemoveItemOrdemServico
{
    public class RemoveItemOrdemServicoHandler : IRequestHandler<RemoveItemOrdemServicoCommand, bool>
    {
        private readonly IOrdemServicoRepository _repository;
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly IContaReceberRepository _contaReceberRepository;

        public RemoveItemOrdemServicoHandler(
            IOrdemServicoRepository repository,
            IEstoqueRepository estoqueRepository,
            IContaReceberRepository contaReceberRepository)
        {
            _repository = repository;
            _estoqueRepository = estoqueRepository;
            _contaReceberRepository = contaReceberRepository;
        }

        public async Task<bool> Handle(RemoveItemOrdemServicoCommand request, CancellationToken cancellationToken)
        {
            var item = await _repository.GetItemByIdAsync(request.ItemId);
            if (item is null) return false;

            var ordemServicoId = item.OrdemServicoId;
            var estoqueId = item.EstoqueId;
            var quantidade = item.Quantidade;

            await _repository.RemoveItemAsync(item);

            if (estoqueId is not null)
            {
                var estoque = await _estoqueRepository.GetByIdAsync(estoqueId.Value);
                if (estoque is not null)
                {
                    estoque.Quantidade += quantidade;
                    await _estoqueRepository.UpdateAsync(estoque);
                }
            }

            var contaExistente = await _contaReceberRepository.GetByOrdemServicoIdAsync(ordemServicoId);
            if (contaExistente is not null)
            {
                var ordemServico = await _repository.GetByIdAsync(ordemServicoId);
                if (ordemServico is not null)
                {
                    var subtotal = ordemServico.ValorMaoDeObra
                        + ordemServico.Itens.Sum(i => i.Quantidade * i.ValorUnitario);
                    var valorTotal = subtotal - subtotal * ordemServico.Desconto / 100m;

                    contaExistente.Valor = valorTotal;
                    await _contaReceberRepository.UpdateAsync(contaExistente);
                }
            }

            return true;
        }
    }
}
