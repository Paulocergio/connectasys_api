using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.Agendamentos.GetAllAgendamentos
{
    public class GetAllAgendamentosQuery : IRequest<List<AgendamentoDto>> { }
}
