using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.OrdensServico.GetOrdemServicoById
{
    public class GetOrdemServicoByIdHandler : IRequestHandler<GetOrdemServicoByIdQuery, OrdemServicoDto?>
    {
        private readonly IOrdemServicoRepository _repository;

        public GetOrdemServicoByIdHandler(IOrdemServicoRepository repository) => _repository = repository;

        public async Task<OrdemServicoDto?> Handle(GetOrdemServicoByIdQuery request, CancellationToken cancellationToken)
        {
            var ordemServico = await _repository.GetByIdAsync(request.Id);
            return ordemServico is null ? null : OrdemServicoDto.DaEntidade(ordemServico);
        }
    }
}
