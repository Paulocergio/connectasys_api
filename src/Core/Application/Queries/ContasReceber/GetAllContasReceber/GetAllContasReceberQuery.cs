using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.ContasReceber.GetAllContasReceber
{
    public class GetAllContasReceberQuery : IRequest<List<ContaReceberDto>> { }
}
