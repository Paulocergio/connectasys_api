using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.Veiculos.GetVeiculoById
{
    public class GetVeiculoByIdQuery : IRequest<VeiculoDto?>
    {
        public int Id { get; set; }
    }
}
