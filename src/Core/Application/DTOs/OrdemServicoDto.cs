using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.DTOs
{
    public class OrdemServicoDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int VeiculoId { get; set; }
        public Guid? TecnicoId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string DescricaoProblema { get; set; } = string.Empty;
        public string? Diagnostico { get; set; }
        public string? Solucao { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataConclusao { get; set; }
        public decimal ValorMaoDeObra { get; set; }
        public decimal Desconto { get; set; }
        public DateTime? AprovacaoClienteEm { get; set; }
        public string? AprovacaoClienteNome { get; set; }
        public List<ItemOrdemServicoDto> Itens { get; set; } = new();
        public decimal ValorTotal { get; set; }

        public static OrdemServicoDto DaEntidade(OrdemServico o)
        {
            var itens = o.Itens.Select(i => new ItemOrdemServicoDto
            {
                Id = i.Id,
                OrdemServicoId = i.OrdemServicoId,
                Descricao = i.Descricao,
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario,
                EstoqueId = i.EstoqueId
            }).ToList();

            var valorItens = itens.Sum(i => i.Quantidade * i.ValorUnitario);
            var subtotal = o.ValorMaoDeObra + valorItens;

            return new OrdemServicoDto
            {
                Id = o.Id,
                ClienteId = o.ClienteId,
                VeiculoId = o.VeiculoId,
                TecnicoId = o.TecnicoId,
                Status = o.Status,
                DescricaoProblema = o.DescricaoProblema,
                Diagnostico = o.Diagnostico,
                Solucao = o.Solucao,
                DataAbertura = o.DataAbertura,
                DataConclusao = o.DataConclusao,
                ValorMaoDeObra = o.ValorMaoDeObra,
                Desconto = o.Desconto,
                AprovacaoClienteEm = o.AprovacaoClienteEm,
                AprovacaoClienteNome = o.AprovacaoClienteNome,
                Itens = itens,
                ValorTotal = subtotal - subtotal * o.Desconto / 100m
            };
        }
    }
}
