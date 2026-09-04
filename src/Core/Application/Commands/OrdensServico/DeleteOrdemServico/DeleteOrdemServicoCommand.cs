using MediatR;

namespace connectasys_api.Core.Application.Commands.OrdensServico.DeleteOrdemServico
{
    public class DeleteOrdemServicoCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
