using connectasys_api.Core.Application.Common;

namespace connectasys_api.Core.Domain.Entities
{
    public class OrdemServico
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int VeiculoId { get; set; }
        public Guid? TecnicoId { get; set; }
        public string Status { get; set; } = StatusOrdemServico.Aberto;
        public string DescricaoProblema { get; set; } = string.Empty;
        public string? Diagnostico { get; set; }
        public string? Solucao { get; set; }
        public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
        public DateTime? DataConclusao { get; set; }
        public decimal ValorMaoDeObra { get; set; }

        /// Percentual de desconto (0 a 100) aplicado sobre mão de obra + itens.
        public decimal Desconto { get; set; }
        public DateTime? AprovacaoClienteEm { get; set; }
        public string? AprovacaoClienteNome { get; set; }

        public List<ItemOrdemServico> Itens { get; set; } = new();
    }
}
