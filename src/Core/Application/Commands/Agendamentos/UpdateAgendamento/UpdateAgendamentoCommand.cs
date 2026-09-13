using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Commands.Agendamentos.UpdateAgendamento
{
    public enum AtualizarAgendamentoResultado
    {
        Sucesso,
        NotFound,
        TecnicoInvalido,
        StatusInvalido,
        Conflito
    }

    public class AtualizarAgendamentoResult
    {
        public AtualizarAgendamentoResultado Resultado { get; set; }
        public AgendamentoDto? Conflitante { get; set; }
    }

    public class UpdateAgendamentoCommand : IRequest<AtualizarAgendamentoResult>
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
    }
}
