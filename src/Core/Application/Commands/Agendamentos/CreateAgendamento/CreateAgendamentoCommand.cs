using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Commands.Agendamentos.CreateAgendamento
{
    public enum CriarAgendamentoResultado
    {
        Sucesso,
        TecnicoInvalido,
        Conflito
    }

    public class CriarAgendamentoResult
    {
        public CriarAgendamentoResultado Resultado { get; set; }
        public AgendamentoDto? Agendamento { get; set; }
        public AgendamentoDto? Conflitante { get; set; }
    }

    public class CreateAgendamentoCommand : IRequest<CriarAgendamentoResult>
    {
        public Guid TecnicoId { get; set; }
        public int? ClienteId { get; set; }
        public int? VeiculoId { get; set; }
        public int? OrdemServicoId { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime? DataHoraFim { get; set; }
        public string? Observacao { get; set; }
    }
}
