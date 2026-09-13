using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.Estoques.GetAllEstoque
{
    public class GetAllEstoqueHandler : IRequestHandler<GetAllEstoqueQuery, List<EstoqueDto>>
    {
        private readonly IEstoqueRepository _repository;

        public GetAllEstoqueHandler(IEstoqueRepository repository) => _repository = repository;

        public async Task<List<EstoqueDto>> Handle(GetAllEstoqueQuery request, CancellationToken cancellationToken)
        {
            var itens = await _repository.GetAllAsync();

            return itens.Select(e => new EstoqueDto
            {
                Id = e.Id,
                Nome = e.Nome,
                Descricao = e.Descricao,
                Quantidade = e.Quantidade,
                PrecoCompra = e.PrecoCompra,
                PrecoVenda = e.PrecoVenda,
                DataCadastro = e.DataCadastro
            }).ToList();
        }
    }
}
