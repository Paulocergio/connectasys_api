using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Commands.OrdensServico.AddItemOrdemServico
{
    public enum AddItemOrdemServicoResultado
    {
        Sucesso,
        OrdemServicoNaoEncontrada,
        EstoqueNaoEncontrado,
        EstoqueInsuficiente
    }

    public class AddItemOrdemServicoResult
    {
        public AddItemOrdemServicoResultado Resultado { get; set; }
        public ItemOrdemServicoDto? Item { get; set; }
    }

    public class AddItemOrdemServicoCommand : IRequest<AddItemOrdemServicoResult>
    {
        public int OrdemServicoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public int? EstoqueId { get; set; }
    }
}
