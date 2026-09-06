using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.Estoques.CreateEstoque
{
    public class CreateEstoqueHandler : IRequestHandler<CreateEstoqueCommand, EstoqueDto>
    {
        private readonly IEstoqueRepository _repository;

        public CreateEstoqueHandler(IEstoqueRepository repository) => _repository = repository;

        public async Task<EstoqueDto> Handle(CreateEstoqueCommand request, CancellationToken cancellationToken)
        {
            var estoque = new Estoque
            {
                Nome = request.Nome,
                Descricao = request.Descricao,
                Quantidade = request.Quantidade,
                PrecoCompra = request.PrecoCompra,
                PrecoVenda = request.PrecoVenda,
                EstoqueMinimo = request.EstoqueMinimo,
                DataCadastro = DateTime.UtcNow
            };

            await _repository.AddAsync(estoque);

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
