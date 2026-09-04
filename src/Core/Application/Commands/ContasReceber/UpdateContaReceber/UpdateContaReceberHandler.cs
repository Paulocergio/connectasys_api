using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.ContasReceber.UpdateContaReceber
{
    public class UpdateContaReceberHandler : IRequestHandler<UpdateContaReceberCommand, UpdateContaReceberResult>
    {
        private readonly IContaReceberRepository _repository;
        private readonly IClienteRepository _clienteRepository;

        public UpdateContaReceberHandler(IContaReceberRepository repository, IClienteRepository clienteRepository)
        {
            _repository = repository;
            _clienteRepository = clienteRepository;
        }

        public async Task<UpdateContaReceberResult> Handle(UpdateContaReceberCommand request, CancellationToken cancellationToken)
        {
            var contaReceber = await _repository.GetByIdAsync(request.Id);
            if (contaReceber is null) return UpdateContaReceberResult.ContaNotFound;

            var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
            if (cliente is null) return UpdateContaReceberResult.ClienteInvalido;

            if (request.DataRecebimento.HasValue && !FormasPagamento.EhValida(request.FormaPagamento))
                return UpdateContaReceberResult.FormaPagamentoInvalida;

            contaReceber.ClienteId = request.ClienteId;
            contaReceber.Descricao = request.Descricao;
            contaReceber.Valor = request.Valor;
            contaReceber.DataVencimento = DateTime.SpecifyKind(request.DataVencimento, DateTimeKind.Utc);
            contaReceber.DataRecebimento = request.DataRecebimento.HasValue
                ? DateTime.SpecifyKind(request.DataRecebimento.Value, DateTimeKind.Utc)
                : null;
            contaReceber.FormaPagamento = request.DataRecebimento.HasValue ? request.FormaPagamento : null;

            await _repository.UpdateAsync(contaReceber);
            return UpdateContaReceberResult.Success;
        }
    }
}
