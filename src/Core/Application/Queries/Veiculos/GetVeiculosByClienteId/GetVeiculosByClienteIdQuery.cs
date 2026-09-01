using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.Veiculos.GetVeiculosByClienteId
{
    public class GetVeiculosByClienteIdQuery : IRequest<List<VeiculoDto>>
    {
        public int ClienteId { get; set; }
    }
}
