using MediatR;

namespace connectasys_api.Core.Application.Commands.Veiculos.DeleteVeiculo
{
    public class DeleteVeiculoCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
