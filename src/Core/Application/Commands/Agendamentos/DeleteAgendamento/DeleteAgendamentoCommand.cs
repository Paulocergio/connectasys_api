using MediatR;

namespace connectasys_api.Core.Application.Commands.Agendamentos.DeleteAgendamento
{
    public class DeleteAgendamentoCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
