using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.ContasPagar.UpdateContaPagar
{
    public class UpdateContaPagarHandler : IRequestHandler<UpdateContaPagarCommand, UpdateContaPagarResult>
    {
        private readonly IContaPagarRepository _repository;

        public UpdateContaPagarHandler(IContaPagarRepository repository) => _repository = repository;

        public async Task<UpdateContaPagarResult> Handle(UpdateContaPagarCommand request, CancellationToken cancellationToken)
        {
            var contaPagar = await _repository.GetByIdAsync(request.Id);
            if (contaPagar is null) return UpdateContaPagarResult.ContaNotFound;

            if (request.DataPagamento.HasValue && !FormasPagamento.EhValida(request.FormaPagamento))
                return UpdateContaPagarResult.FormaPagamentoInvalida;

            contaPagar.Descricao = request.Descricao;
            contaPagar.Fornecedor = request.Fornecedor;
            contaPagar.Valor = request.Valor;
            contaPagar.DataVencimento = DateTime.SpecifyKind(request.DataVencimento, DateTimeKind.Utc);
            contaPagar.DataPagamento = request.DataPagamento.HasValue
                ? DateTime.SpecifyKind(request.DataPagamento.Value, DateTimeKind.Utc)
                : null;
            contaPagar.FormaPagamento = request.DataPagamento.HasValue ? request.FormaPagamento : null;

            await _repository.UpdateAsync(contaPagar);
            return UpdateContaPagarResult.Success;
        }
    }
}
