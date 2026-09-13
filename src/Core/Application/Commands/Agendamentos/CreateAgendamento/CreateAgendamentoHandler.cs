using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.Agendamentos.CreateAgendamento
{
    public class CreateAgendamentoHandler : IRequestHandler<CreateAgendamentoCommand, CriarAgendamentoResult>
    {
        private readonly IAgendamentoRepository _repository;
        private readonly IUsuarioRepository _usuarioRepository;

        public CreateAgendamentoHandler(IAgendamentoRepository repository, IUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<CriarAgendamentoResult> Handle(CreateAgendamentoCommand request, CancellationToken cancellationToken)
        {
            var tecnico = await _usuarioRepository.GetByIdAsync(request.TecnicoId);
            if (tecnico is null || tecnico.Role != Roles.Mecanico)
                return new CriarAgendamentoResult { Resultado = CriarAgendamentoResultado.TecnicoInvalido };

            // Mesmo tratamento de Kind da UtcDateTimeConverter (AppDbContext) —
            // datas recebidas do corpo da requisição podem chegar com
            // Kind=Unspecified, o que o Npgsql rejeita em comparação com
            // coluna timestamptz.
            var dataHoraUtc = request.DataHoraInicio.Kind == DateTimeKind.Utc
                ? request.DataHoraInicio
                : DateTime.SpecifyKind(request.DataHoraInicio, DateTimeKind.Utc);
            var slot = SlotAgendamento.Truncar(dataHoraUtc);

            var conflito = await _repository.GetConflitoAsync(request.TecnicoId, slot);
            if (conflito is not null)
                return new CriarAgendamentoResult
                {
                    Resultado = CriarAgendamentoResultado.Conflito,
                    Conflitante = AgendamentoDto.DaEntidade(conflito)
                };

            var agendamento = new Agendamento
            {
                TecnicoId = request.TecnicoId,
                ClienteId = request.ClienteId,
                VeiculoId = request.VeiculoId,
                OrdemServicoId = request.OrdemServicoId,
                DataHoraInicio = slot,
                DataHoraFim = request.DataHoraFim,
                Observacao = request.Observacao,
                DataCadastro = DateTime.UtcNow
            };

            await _repository.AddAsync(agendamento);

            return new CriarAgendamentoResult
            {
                Resultado = CriarAgendamentoResultado.Sucesso,
                Agendamento = AgendamentoDto.DaEntidade(agendamento)
            };
        }
    }
}
