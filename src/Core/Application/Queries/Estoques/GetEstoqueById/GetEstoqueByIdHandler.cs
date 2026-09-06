using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.Estoques.GetEstoqueById
{
    public class GetEstoqueByIdHandler : IRequestHandler<GetEstoqueByIdQuery, EstoqueDto?>
    {
        private readonly IEstoqueRepository _repository;

        public GetEstoqueByIdHandler(IEstoqueRepository repository) => _repository = repository;

        public async Task<EstoqueDto?> Handle(GetEstoqueByIdQuery request, CancellationToken cancellationToken)
        {
            var estoque = await _repository.GetByIdAsync(request.Id);
            if (estoque is null) return null;

            return new EstoqueDto
            {
                Id = estoque.Id,
                Nome = estoque.Nome,
                Descricao = estoque.Descricao,
                Quantidade = estoque.Quantidade,
                PrecoCompra = estoque.PrecoCompra,
                PrecoVenda = estoque.PrecoVenda,
                EstoqueMinimo = estoque.EstoqueMinimo,
                DataCadastro = estoque.DataCadastro
            };
        }
    }
}
