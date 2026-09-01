namespace connectasys_api.Core.Domain.Entities
{
    public class ContaPagar
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Fornecedor { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataPagamento { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}
