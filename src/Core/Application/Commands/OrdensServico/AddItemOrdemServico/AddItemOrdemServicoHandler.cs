using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.OrdensServico.AddItemOrdemServico
{
    public class AddItemOrdemServicoHandler : IRequestHandler<AddItemOrdemServicoCommand, ItemOrdemServicoDto?>
    {
        private readonly IOrdemServicoRepository _repository;

        public AddItemOrdemServicoHandler(IOrdemServicoRepository repository) => _repository = repository;

        public async Task<ItemOrdemServicoDto?> Handle(AddItemOrdemServicoCommand request, CancellationToken cancellationToken)
        {
            var ordemServico = await _repository.GetByIdAsync(request.OrdemServicoId);
            if (ordemServico is null) return null;

            var item = new ItemOrdemServico
            {
                OrdemServicoId = request.OrdemServicoId,
                Descricao = request.Descricao,
                Quantidade = request.Quantidade,
                ValorUnitario = request.ValorUnitario
            };

            await _repository.AddItemAsync(item);

            return new ItemOrdemServicoDto
            {
                Id = item.Id,
                OrdemServicoId = item.OrdemServicoId,
                Descricao = item.Descricao,
                Quantidade = item.Quantidade,
                ValorUnitario = item.ValorUnitario
            };
        }
    }
}
