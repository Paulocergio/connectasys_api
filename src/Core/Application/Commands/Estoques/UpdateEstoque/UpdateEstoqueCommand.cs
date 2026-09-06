using MediatR;

namespace connectasys_api.Core.Application.Commands.Estoques.UpdateEstoque
{
    public enum UpdateEstoqueResult
    {
        Success,
        NotFound
    }

    public class UpdateEstoqueCommand : IRequest<UpdateEstoqueResult>
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PrecoCompra { get; set; }
        public decimal PrecoVenda { get; set; }
        public decimal EstoqueMinimo { get; set; }
    }
}
