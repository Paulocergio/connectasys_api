using MediatR;

namespace connectasys_api.Core.Application.Commands.ContasPagar.UpdateContaPagar
{
    public class UpdateContaPagarCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Fornecedor { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataPagamento { get; set; }
    }
}
