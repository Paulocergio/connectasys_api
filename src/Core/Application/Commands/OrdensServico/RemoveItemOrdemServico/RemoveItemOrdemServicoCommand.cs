using MediatR;

namespace connectasys_api.Core.Application.Commands.OrdensServico.RemoveItemOrdemServico
{
    public class RemoveItemOrdemServicoCommand : IRequest<bool>
    {
        public int ItemId { get; set; }
    }
}
