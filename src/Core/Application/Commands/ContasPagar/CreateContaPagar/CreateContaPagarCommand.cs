using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Commands.ContasPagar.CreateContaPagar
{
    public class CreateContaPagarCommand : IRequest<ContaPagarDto>
    {
        public string Descricao { get; set; } = string.Empty;
        public string Fornecedor { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
    }
}
