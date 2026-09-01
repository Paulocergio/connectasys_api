namespace connectasys_api.Core.Application.DTOs
{
    public class ContaPagarDto
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Fornecedor { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataPagamento { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
    }
}
