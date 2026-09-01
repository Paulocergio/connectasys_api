using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.ContasPagar.GetContaPagarById
{
    public class GetContaPagarByIdHandler : IRequestHandler<GetContaPagarByIdQuery, ContaPagarDto?>
    {
        private readonly IContaPagarRepository _repository;

        public GetContaPagarByIdHandler(IContaPagarRepository repository) => _repository = repository;

        public async Task<ContaPagarDto?> Handle(GetContaPagarByIdQuery request, CancellationToken cancellationToken)
        {
            var conta = await _repository.GetByIdAsync(request.Id);
            if (conta is null) return null;

            return new ContaPagarDto
            {
                Id = conta.Id,
                Descricao = conta.Descricao,
                Fornecedor = conta.Fornecedor,
                Valor = conta.Valor,
                DataVencimento = conta.DataVencimento,
                DataPagamento = conta.DataPagamento,
                Status = StatusConta.Calcular(conta.DataPagamento, conta.DataVencimento),
                DataCadastro = conta.DataCadastro
            };
        }
    }
}
