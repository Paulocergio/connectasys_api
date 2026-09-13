namespace connectasys_api.Core.Domain.Entities
{
    public class ItemOrdemServico
    {
        public int Id { get; set; }
        public Guid EmpresaId { get; set; }
        public int OrdemServicoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public int? EstoqueId { get; set; }
    }
}
