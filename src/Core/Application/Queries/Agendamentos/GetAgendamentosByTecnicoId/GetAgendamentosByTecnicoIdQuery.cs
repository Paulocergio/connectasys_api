using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.Agendamentos.GetAgendamentosByTecnicoId
{
    public class GetAgendamentosByTecnicoIdQuery : IRequest<List<AgendamentoDto>>
    {
        public Guid TecnicoId { get; set; }
        public DateTime? Data { get; set; }
    }
}
