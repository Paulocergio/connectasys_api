using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Commands.ContasReceber.CreateContaReceber
{
    public class CreateContaReceberCommand : IRequest<ContaReceberDto?>
    {
        public int ClienteId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
    }
}
