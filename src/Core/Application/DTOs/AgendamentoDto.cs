using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.DTOs
{
    public class AgendamentoDto
    {
        public int Id { get; set; }
        public Guid TecnicoId { get; set; }
        public int? ClienteId { get; set; }
        public int? VeiculoId { get; set; }
        public int? OrdemServicoId { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime? DataHoraFim { get; set; }
        public string? Observacao { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }

        public static AgendamentoDto DaEntidade(Agendamento a) => new()
        {
            Id = a.Id,
            TecnicoId = a.TecnicoId,
            ClienteId = a.ClienteId,
            VeiculoId = a.VeiculoId,
            OrdemServicoId = a.OrdemServicoId,
            DataHoraInicio = a.DataHoraInicio,
            DataHoraFim = a.DataHoraFim,
            Observacao = a.Observacao,
            Status = a.Status,
            DataCadastro = a.DataCadastro
        };
    }
}
