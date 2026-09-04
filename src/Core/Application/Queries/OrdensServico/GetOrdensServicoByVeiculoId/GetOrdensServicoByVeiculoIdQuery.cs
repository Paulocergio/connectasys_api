using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.OrdensServico.GetOrdensServicoByVeiculoId
{
    public class GetOrdensServicoByVeiculoIdQuery : IRequest<List<OrdemServicoDto>>
    {
        public int VeiculoId { get; set; }
    }
}
