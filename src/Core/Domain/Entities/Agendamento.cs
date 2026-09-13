using connectasys_api.Core.Application.Common;

namespace connectasys_api.Core.Domain.Entities
{
    public class Agendamento
    {
        public int Id { get; set; }
        public Guid TecnicoId { get; set; }
        public int? ClienteId { get; set; }
        public int? VeiculoId { get; set; }
        public int? OrdemServicoId { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime? DataHoraFim { get; set; }
        public string? Observacao { get; set; }
        public string Status { get; set; } = StatusAgendamento.Agendado;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}
