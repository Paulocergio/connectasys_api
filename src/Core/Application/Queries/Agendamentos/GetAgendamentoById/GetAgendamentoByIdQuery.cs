using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.Agendamentos.GetAgendamentoById
{
    public class GetAgendamentoByIdQuery : IRequest<AgendamentoDto?>
    {
        public int Id { get; set; }
    }
}
