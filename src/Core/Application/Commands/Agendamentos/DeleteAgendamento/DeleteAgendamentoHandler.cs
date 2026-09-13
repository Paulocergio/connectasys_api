using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.Agendamentos.DeleteAgendamento
{
    public class DeleteAgendamentoHandler : IRequestHandler<DeleteAgendamentoCommand, bool>
    {
        private readonly IAgendamentoRepository _repository;

        public DeleteAgendamentoHandler(IAgendamentoRepository repository) => _repository = repository;

        public async Task<bool> Handle(DeleteAgendamentoCommand request, CancellationToken cancellationToken)
        {
            var agendamento = await _repository.GetByIdAsync(request.Id);
            if (agendamento is null) return false;

            await _repository.DeleteAsync(agendamento);
            return true;
        }
    }
}
