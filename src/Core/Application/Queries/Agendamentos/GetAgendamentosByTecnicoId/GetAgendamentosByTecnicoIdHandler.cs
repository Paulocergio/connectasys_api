using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.Agendamentos.GetAgendamentosByTecnicoId
{
    public class GetAgendamentosByTecnicoIdHandler : IRequestHandler<GetAgendamentosByTecnicoIdQuery, List<AgendamentoDto>>
    {
        private readonly IAgendamentoRepository _repository;

        public GetAgendamentosByTecnicoIdHandler(IAgendamentoRepository repository) => _repository = repository;

        public async Task<List<AgendamentoDto>> Handle(GetAgendamentosByTecnicoIdQuery request, CancellationToken cancellationToken)
        {
            var agendamentos = await _repository.GetByTecnicoIdAsync(request.TecnicoId, request.Data);
            return agendamentos.Select(AgendamentoDto.DaEntidade).ToList();
        }
    }
}
