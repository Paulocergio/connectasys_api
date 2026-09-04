using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.ContasPagar.GetAllContasPagar
{
    public class GetAllContasPagarHandler : IRequestHandler<GetAllContasPagarQuery, List<ContaPagarDto>>
    {
        private readonly IContaPagarRepository _repository;

        public GetAllContasPagarHandler(IContaPagarRepository repository) => _repository = repository;

        public async Task<List<ContaPagarDto>> Handle(GetAllContasPagarQuery request, CancellationToken cancellationToken)
        {
            var contas = await _repository.GetAllAsync();

            return contas.Select(c => new ContaPagarDto
            {
                Id = c.Id,
                Descricao = c.Descricao,
                Fornecedor = c.Fornecedor,
                Valor = c.Valor,
                DataVencimento = c.DataVencimento,
                DataPagamento = c.DataPagamento,
                FormaPagamento = c.FormaPagamento,
                Status = StatusConta.Calcular(c.DataPagamento, c.DataVencimento),
                DataCadastro = c.DataCadastro
            }).ToList();
        }
    }
}
