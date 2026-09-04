using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.OrdensServico.GetOrdensServicoByVeiculoId
{
    public class GetOrdensServicoByVeiculoIdHandler : IRequestHandler<GetOrdensServicoByVeiculoIdQuery, List<OrdemServicoDto>>
    {
        private readonly IOrdemServicoRepository _repository;

        public GetOrdensServicoByVeiculoIdHandler(IOrdemServicoRepository repository) => _repository = repository;

        public async Task<List<OrdemServicoDto>> Handle(GetOrdensServicoByVeiculoIdQuery request, CancellationToken cancellationToken)
        {
            var ordensServico = await _repository.GetByVeiculoIdAsync(request.VeiculoId);
            return ordensServico.Select(OrdemServicoDto.DaEntidade).ToList();
        }
    }
}
