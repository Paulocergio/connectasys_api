using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.ContasReceber.GetContaReceberById
{
    public class GetContaReceberByIdQuery : IRequest<ContaReceberDto?>
    {
        public int Id { get; set; }
    }
}
