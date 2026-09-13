using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.Agendamentos.GetConflitoAgendamento
{
    public class GetConflitoAgendamentoQuery : IRequest<AgendamentoDto?>
    {
        public Guid TecnicoId { get; set; }
        public DateTime DataHora { get; set; }
    }
}
