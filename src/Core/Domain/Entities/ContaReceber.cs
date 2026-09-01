namespace connectasys_api.Core.Domain.Entities
{
    public class ContaReceber
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataRecebimento { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}
