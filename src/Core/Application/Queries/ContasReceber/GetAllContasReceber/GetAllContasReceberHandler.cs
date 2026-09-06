using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.ContasReceber.GetAllContasReceber
{
    public class GetAllContasReceberHandler : IRequestHandler<GetAllContasReceberQuery, List<ContaReceberDto>>
    {
        private readonly IContaReceberRepository _repository;

        public GetAllContasReceberHandler(IContaReceberRepository repository) => _repository = repository;

        public async Task<List<ContaReceberDto>> Handle(GetAllContasReceberQuery request, CancellationToken cancellationToken)
        {
            var contas = await _repository.GetAllAsync();

            return contas.Select(c => new ContaReceberDto
            {
                Id = c.Id,
                ClienteId = c.ClienteId,
                Descricao = c.Descricao,
                Valor = c.Valor,
                DataVencimento = c.DataVencimento,
                DataRecebimento = c.DataRecebimento,
                FormaPagamento = c.FormaPagamento,
                Status = StatusConta.Calcular(c.DataRecebimento, c.DataVencimento),
                DataCadastro = c.DataCadastro,
                OrdemServicoId = c.OrdemServicoId
            }).ToList();
        }
    }
}
