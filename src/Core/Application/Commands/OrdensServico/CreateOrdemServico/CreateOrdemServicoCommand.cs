using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Commands.OrdensServico.CreateOrdemServico
{
    public class CreateOrdemServicoCommand : IRequest<OrdemServicoDto?>
    {
        public int ClienteId { get; set; }
        public int VeiculoId { get; set; }
        public Guid? TecnicoId { get; set; }
        public string DescricaoProblema { get; set; } = string.Empty;
        public DateTime? PrevisaoTermino { get; set; }
        public decimal ValorMaoDeObra { get; set; }
        public decimal Desconto { get; set; }
    }
}
