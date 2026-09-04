using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.OrdensServico.GetOrdensServicoByClienteId
{
    public class GetOrdensServicoByClienteIdHandler : IRequestHandler<GetOrdensServicoByClienteIdQuery, List<OrdemServicoDto>>
    {
        private readonly IOrdemServicoRepository _repository;

        public GetOrdensServicoByClienteIdHandler(IOrdemServicoRepository repository) => _repository = repository;

        public async Task<List<OrdemServicoDto>> Handle(GetOrdensServicoByClienteIdQuery request, CancellationToken cancellationToken)
        {
            var ordensServico = await _repository.GetByClienteIdAsync(request.ClienteId);
            return ordensServico.Select(OrdemServicoDto.DaEntidade).ToList();
        }
    }
}
