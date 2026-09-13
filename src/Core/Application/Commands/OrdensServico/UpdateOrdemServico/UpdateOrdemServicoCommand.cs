using MediatR;

namespace connectasys_api.Core.Application.Commands.OrdensServico.UpdateOrdemServico
{
    public enum UpdateOrdemServicoResult
    {
        Success,
        OrdemServicoNotFound,
        ClienteOuVeiculoInvalido,
        TecnicoInvalido,
        StatusInvalido
    }

    public class UpdateOrdemServicoCommand : IRequest<UpdateOrdemServicoResult>
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int VeiculoId { get; set; }
        public Guid? TecnicoId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string DescricaoProblema { get; set; } = string.Empty;
        public string? Diagnostico { get; set; }
        public string? Solucao { get; set; }
        public DateTime? DataConclusao { get; set; }
        public decimal ValorMaoDeObra { get; set; }
        public decimal Desconto { get; set; }
        public DateTime? AprovacaoClienteEm { get; set; }
        public string? AprovacaoClienteNome { get; set; }
    }
}
