using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Commands.OrdensServico.AddItemOrdemServico
{
    public class AddItemOrdemServicoCommand : IRequest<ItemOrdemServicoDto?>
    {
        public int OrdemServicoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
    }
}
