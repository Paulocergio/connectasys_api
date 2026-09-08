using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.OrdensServico.AddItemOrdemServico
{
    public class AddItemOrdemServicoHandler : IRequestHandler<AddItemOrdemServicoCommand, AddItemOrdemServicoResult>
    {
        private readonly IOrdemServicoRepository _repository;
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly IContaReceberRepository _contaReceberRepository;

        public AddItemOrdemServicoHandler(
            IOrdemServicoRepository repository,
            IEstoqueRepository estoqueRepository,
            IContaReceberRepository contaReceberRepository)
        {
            _repository = repository;
            _estoqueRepository = estoqueRepository;
            _contaReceberRepository = contaReceberRepository;
        }

        public async Task<AddItemOrdemServicoResult> Handle(AddItemOrdemServicoCommand request, CancellationToken cancellationToken)
        {
            var ordemServico = await _repository.GetByIdAsync(request.OrdemServicoId);
            if (ordemServico is null)
                return new AddItemOrdemServicoResult { Resultado = AddItemOrdemServicoResultado.OrdemServicoNaoEncontrada };

            var descricao = request.Descricao;
            var valorUnitario = request.ValorUnitario;
            Estoque? estoque = null;

            if (request.EstoqueId is not null)
            {
                estoque = await _estoqueRepository.GetByIdAsync(request.EstoqueId.Value);
                if (estoque is null)
                    return new AddItemOrdemServicoResult { Resultado = AddItemOrdemServicoResultado.EstoqueNaoEncontrado };

                if (estoque.Quantidade < request.Quantidade)
                    return new AddItemOrdemServicoResult { Resultado = AddItemOrdemServicoResultado.EstoqueInsuficiente };

                descricao = estoque.Nome;
                valorUnitario = estoque.PrecoVenda;
            }

            var item = new ItemOrdemServico
            {
                OrdemServicoId = request.OrdemServicoId,
                Descricao = descricao,
                Quantidade = request.Quantidade,
                ValorUnitario = valorUnitario,
                EstoqueId = request.EstoqueId
            };

            await _repository.AddItemAsync(item);

            if (estoque is not null)
            {
                estoque.Quantidade -= request.Quantidade;
                await _estoqueRepository.UpdateAsync(estoque);
            }

            var contaExistente = await _contaReceberRepository.GetByOrdemServicoIdAsync(ordemServico.Id);
            if (contaExistente is not null)
            {
                var ordemServicoAtualizada = await _repository.GetByIdAsync(ordemServico.Id);
                var subtotal = ordemServicoAtualizada!.ValorMaoDeObra
                    + ordemServicoAtualizada.Itens.Sum(i => i.Quantidade * i.ValorUnitario);
                var valorTotal = subtotal - subtotal * ordemServicoAtualizada.Desconto / 100m;

                contaExistente.Valor = valorTotal;
                await _contaReceberRepository.UpdateAsync(contaExistente);
            }

            return new AddItemOrdemServicoResult
            {
                Resultado = AddItemOrdemServicoResultado.Sucesso,
                Item = new ItemOrdemServicoDto
                {
                    Id = item.Id,
                    OrdemServicoId = item.OrdemServicoId,
                    Descricao = item.Descricao,
                    Quantidade = item.Quantidade,
                    ValorUnitario = item.ValorUnitario,
                    EstoqueId = item.EstoqueId
                }
            };
        }
    }
}
