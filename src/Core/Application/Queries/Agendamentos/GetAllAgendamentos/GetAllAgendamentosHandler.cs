using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.Agendamentos.GetAllAgendamentos
{
    public class GetAllAgendamentosHandler : IRequestHandler<GetAllAgendamentosQuery, List<AgendamentoDto>>
    {
        private readonly IAgendamentoRepository _repository;

        public GetAllAgendamentosHandler(IAgendamentoRepository repository) => _repository = repository;

        public async Task<List<AgendamentoDto>> Handle(GetAllAgendamentosQuery request, CancellationToken cancellationToken)
        {
            var agendamentos = await _repository.GetAllAsync();
            return agendamentos.Select(AgendamentoDto.DaEntidade).ToList();
        }
    }
}
