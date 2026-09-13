using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.Agendamentos.GetConflitoAgendamento
{
    public class GetConflitoAgendamentoHandler : IRequestHandler<GetConflitoAgendamentoQuery, AgendamentoDto?>
    {
        private readonly IAgendamentoRepository _repository;

        public GetConflitoAgendamentoHandler(IAgendamentoRepository repository) => _repository = repository;

        public async Task<AgendamentoDto?> Handle(GetConflitoAgendamentoQuery request, CancellationToken cancellationToken)
        {
            // Mesmo tratamento de Kind da UtcDateTimeConverter (AppDbContext) —
            // datas recebidas de query string podem chegar com Kind=Unspecified,
            // o que o Npgsql rejeita em comparação com coluna timestamptz.
            var dataHoraUtc = request.DataHora.Kind == DateTimeKind.Utc
                ? request.DataHora
                : DateTime.SpecifyKind(request.DataHora, DateTimeKind.Utc);

            var slot = SlotAgendamento.Truncar(dataHoraUtc);
            var conflito = await _repository.GetConflitoAsync(request.TecnicoId, slot);
            return conflito is null ? null : AgendamentoDto.DaEntidade(conflito);
        }
    }
}
