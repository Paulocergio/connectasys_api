using MediatR;

namespace connectasys_api.Core.Application.Commands.ContasReceber.UpdateContaReceber
{
    public enum UpdateContaReceberResult
    {
        Success,
        ContaNotFound,
        ClienteInvalido
    }

    public class UpdateContaReceberCommand : IRequest<UpdateContaReceberResult>
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataRecebimento { get; set; }
    }
}
