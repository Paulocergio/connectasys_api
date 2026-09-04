namespace connectasys_api.Core.Application.DTOs
{
    public class ContaReceberDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataRecebimento { get; set; }
        public string? FormaPagamento { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
    }
}
