namespace connectasys_api.Core.Application.DTOs
{
    public class ItemOrdemServicoDto
    {
        public int Id { get; set; }
        public int OrdemServicoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
    }
}
