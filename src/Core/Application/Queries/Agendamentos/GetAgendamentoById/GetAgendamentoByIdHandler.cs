using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.Agendamentos.GetAgendamentoById
{
    public class GetAgendamentoByIdHandler : IRequestHandler<GetAgendamentoByIdQuery, AgendamentoDto?>
    {
        private readonly IAgendamentoRepository _repository;

        public GetAgendamentoByIdHandler(IAgendamentoRepository repository) => _repository = repository;

        public async Task<AgendamentoDto?> Handle(GetAgendamentoByIdQuery request, CancellationToken cancellationToken)
        {
            var agendamento = await _repository.GetByIdAsync(request.Id);
            return agendamento is null ? null : AgendamentoDto.DaEntidade(agendamento);
        }
    }
}
