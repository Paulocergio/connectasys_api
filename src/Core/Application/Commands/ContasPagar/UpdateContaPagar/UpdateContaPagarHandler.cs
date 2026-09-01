using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.ContasPagar.UpdateContaPagar
{
    public class UpdateContaPagarHandler : IRequestHandler<UpdateContaPagarCommand, bool>
    {
        private readonly IContaPagarRepository _repository;

        public UpdateContaPagarHandler(IContaPagarRepository repository) => _repository = repository;

        public async Task<bool> Handle(UpdateContaPagarCommand request, CancellationToken cancellationToken)
        {
            var contaPagar = await _repository.GetByIdAsync(request.Id);
            if (contaPagar is null) return false;

            contaPagar.Descricao = request.Descricao;
            contaPagar.Fornecedor = request.Fornecedor;
            contaPagar.Valor = request.Valor;
            contaPagar.DataVencimento = request.DataVencimento;
            contaPagar.DataPagamento = request.DataPagamento;

            await _repository.UpdateAsync(contaPagar);
            return true;
        }
    }
}
