using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.Estoques.GetAllEstoque
{
    public class GetAllEstoqueQuery : IRequest<List<EstoqueDto>> { }
}
