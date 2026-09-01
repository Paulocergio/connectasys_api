using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.ContasPagar.GetContaPagarById
{
    public class GetContaPagarByIdQuery : IRequest<ContaPagarDto?>
    {
        public int Id { get; set; }
    }
}
