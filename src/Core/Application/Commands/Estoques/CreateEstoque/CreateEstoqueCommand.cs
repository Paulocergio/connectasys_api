using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Commands.Estoques.CreateEstoque
{
    public class CreateEstoqueCommand : IRequest<EstoqueDto>
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PrecoCompra { get; set; }
        public decimal PrecoVenda { get; set; }
    }
}
