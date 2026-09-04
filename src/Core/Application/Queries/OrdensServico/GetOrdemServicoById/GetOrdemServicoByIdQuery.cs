using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.OrdensServico.GetOrdemServicoById
{
    public class GetOrdemServicoByIdQuery : IRequest<OrdemServicoDto?>
    {
        public int Id { get; set; }
    }
}
