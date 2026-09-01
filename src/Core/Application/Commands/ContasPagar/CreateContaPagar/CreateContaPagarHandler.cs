using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.ContasPagar.CreateContaPagar
{
    public class CreateContaPagarHandler : IRequestHandler<CreateContaPagarCommand, ContaPagarDto>
    {
        private readonly IContaPagarRepository _repository;

        public CreateContaPagarHandler(IContaPagarRepository repository) => _repository = repository;

        public async Task<ContaPagarDto> Handle(CreateContaPagarCommand request, CancellationToken cancellationToken)
        {
            var contaPagar = new ContaPagar
            {
                Descricao = request.Descricao,
                Fornecedor = request.Fornecedor,
                Valor = request.Valor,
                DataVencimento = request.DataVencimento,
                DataCadastro = DateTime.UtcNow
            };

            await _repository.AddAsync(contaPagar);

            return new ContaPagarDto
            {
                Id = contaPagar.Id,
                Descricao = contaPagar.Descricao,
                Fornecedor = contaPagar.Fornecedor,
                Valor = contaPagar.Valor,
                DataVencimento = contaPagar.DataVencimento,
                DataPagamento = contaPagar.DataPagamento,
                Status = StatusConta.Calcular(contaPagar.DataPagamento, contaPagar.DataVencimento),
                DataCadastro = contaPagar.DataCadastro
            };
        }
    }
}
