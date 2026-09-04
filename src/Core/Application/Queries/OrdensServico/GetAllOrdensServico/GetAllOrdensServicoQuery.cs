using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.OrdensServico.GetAllOrdensServico
{
    public class GetAllOrdensServicoQuery : IRequest<List<OrdemServicoDto>>
    {
    }
}
