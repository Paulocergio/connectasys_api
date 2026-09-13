using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.Estoques.UpdateEstoque
{
    public class UpdateEstoqueHandler : IRequestHandler<UpdateEstoqueCommand, UpdateEstoqueResult>
    {
        private readonly IEstoqueRepository _repository;

        public UpdateEstoqueHandler(IEstoqueRepository repository) => _repository = repository;

        public async Task<UpdateEstoqueResult> Handle(UpdateEstoqueCommand request, CancellationToken cancellationToken)
        {
            var estoque = await _repository.GetByIdAsync(request.Id);
            if (estoque is null) return UpdateEstoqueResult.NotFound;

            estoque.Nome = request.Nome;
            estoque.Descricao = request.Descricao;
            estoque.Quantidade = request.Quantidade;
            estoque.PrecoCompra = request.PrecoCompra;
            estoque.PrecoVenda = request.PrecoVenda;

            await _repository.UpdateAsync(estoque);
            return UpdateEstoqueResult.Success;
        }
    }
}
