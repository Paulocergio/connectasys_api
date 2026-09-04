using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.OrdensServico.GetOrdensServicoByClienteId
{
    public class GetOrdensServicoByClienteIdQuery : IRequest<List<OrdemServicoDto>>
    {
        public int ClienteId { get; set; }
    }
}
