namespace connectasys_api.Core.Application.DTOs
{
    public class EstoqueDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PrecoCompra { get; set; }
        public decimal PrecoVenda { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
