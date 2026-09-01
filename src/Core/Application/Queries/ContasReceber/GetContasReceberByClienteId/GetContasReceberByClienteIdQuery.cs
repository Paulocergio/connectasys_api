using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.ContasReceber.GetContasReceberByClienteId
{
    public class GetContasReceberByClienteIdQuery : IRequest<List<ContaReceberDto>>
    {
        public int ClienteId { get; set; }
    }
}
