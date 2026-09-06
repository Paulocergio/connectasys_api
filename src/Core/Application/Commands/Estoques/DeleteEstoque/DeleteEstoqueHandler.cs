using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.Estoques.DeleteEstoque
{
    public class DeleteEstoqueHandler : IRequestHandler<DeleteEstoqueCommand, bool>
    {
        private readonly IEstoqueRepository _repository;

        public DeleteEstoqueHandler(IEstoqueRepository repository) => _repository = repository;

        public async Task<bool> Handle(DeleteEstoqueCommand request, CancellationToken cancellationToken)
        {
            var estoque = await _repository.GetByIdAsync(request.Id);
            if (estoque is null) return false;

            await _repository.DeleteAsync(estoque);
            return true;
        }
    }
}
