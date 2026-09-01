using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.ContasReceber.GetContaReceberById
{
    public class GetContaReceberByIdHandler : IRequestHandler<GetContaReceberByIdQuery, ContaReceberDto?>
    {
        private readonly IContaReceberRepository _repository;

        public GetContaReceberByIdHandler(IContaReceberRepository repository) => _repository = repository;

        public async Task<ContaReceberDto?> Handle(GetContaReceberByIdQuery request, CancellationToken cancellationToken)
        {
            var conta = await _repository.GetByIdAsync(request.Id);
            if (conta is null) return null;

            return new ContaReceberDto
            {
                Id = conta.Id,
                ClienteId = conta.ClienteId,
                Descricao = conta.Descricao,
                Valor = conta.Valor,
                DataVencimento = conta.DataVencimento,
                DataRecebimento = conta.DataRecebimento,
                Status = StatusConta.Calcular(conta.DataRecebimento, conta.DataVencimento),
                DataCadastro = conta.DataCadastro
            };
        }
    }
}
