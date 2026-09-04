using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.OrdensServico.GetAllOrdensServico
{
    public class GetAllOrdensServicoHandler : IRequestHandler<GetAllOrdensServicoQuery, List<OrdemServicoDto>>
    {
        private readonly IOrdemServicoRepository _repository;

        public GetAllOrdensServicoHandler(IOrdemServicoRepository repository) => _repository = repository;

        public async Task<List<OrdemServicoDto>> Handle(GetAllOrdensServicoQuery request, CancellationToken cancellationToken)
        {
            var ordensServico = await _repository.GetAllAsync();
            return ordensServico.Select(OrdemServicoDto.DaEntidade).ToList();
        }
    }
}
