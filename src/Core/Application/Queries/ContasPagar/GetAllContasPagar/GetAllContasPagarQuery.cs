using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.ContasPagar.GetAllContasPagar
{
    public class GetAllContasPagarQuery : IRequest<List<ContaPagarDto>> { }
}
