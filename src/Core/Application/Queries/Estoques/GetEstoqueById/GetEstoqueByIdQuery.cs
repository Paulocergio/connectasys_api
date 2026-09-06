using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.Estoques.GetEstoqueById
{
    public class GetEstoqueByIdQuery : IRequest<EstoqueDto?>
    {
        public int Id { get; set; }
    }
}
