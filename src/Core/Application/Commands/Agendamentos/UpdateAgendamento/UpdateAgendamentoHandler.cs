using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.Agendamentos.UpdateAgendamento
{
    public class UpdateAgendamentoHandler : IRequestHandler<UpdateAgendamentoCommand, AtualizarAgendamentoResult>
    {
        private readonly IAgendamentoRepository _repository;
        private readonly IUsuarioRepository _usuarioRepository;

        public UpdateAgendamentoHandler(IAgendamentoRepository repository, IUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<AtualizarAgendamentoResult> Handle(UpdateAgendamentoCommand request, CancellationToken cancellationToken)
        {
            var agendamento = await _repository.GetByIdAsync(request.Id);
            if (agendamento is null)
                return new AtualizarAgendamentoResult { Resultado = AtualizarAgendamentoResultado.NotFound };

            if (!StatusAgendamento.EhValido(request.Status))
                return new AtualizarAgendamentoResult { Resultado = AtualizarAgendamentoResultado.StatusInvalido };

            var tecnico = await _usuarioRepository.GetByIdAsync(request.TecnicoId);
            if (tecnico is null || tecnico.Role != Roles.Mecanico)
                return new AtualizarAgendamentoResult { Resultado = AtualizarAgendamentoResultado.TecnicoInvalido };

            var dataHoraUtc = request.DataHoraInicio.Kind == DateTimeKind.Utc
                ? request.DataHoraInicio
                : DateTime.SpecifyKind(request.DataHoraInicio, DateTimeKind.Utc);
            var slot = SlotAgendamento.Truncar(dataHoraUtc);

            if (request.Status == StatusAgendamento.Agendado)
            {
                var conflito = await _repository.GetConflitoAsync(request.TecnicoId, slot, ignorarId: agendamento.Id);
                if (conflito is not null)
                    return new AtualizarAgendamentoResult
                    {
                        Resultado = AtualizarAgendamentoResultado.Conflito,
                        Conflitante = AgendamentoDto.DaEntidade(conflito)
                    };
            }

            agendamento.TecnicoId = request.TecnicoId;
            agendamento.ClienteId = request.ClienteId;
            agendamento.VeiculoId = request.VeiculoId;
            agendamento.OrdemServicoId = request.OrdemServicoId;
            agendamento.DataHoraInicio = slot;
            agendamento.DataHoraFim = request.DataHoraFim;
            agendamento.Observacao = request.Observacao;
            agendamento.Status = request.Status;

            await _repository.UpdateAsync(agendamento);
            return new AtualizarAgendamentoResult { Resultado = AtualizarAgendamentoResultado.Sucesso };
        }
    }
}
