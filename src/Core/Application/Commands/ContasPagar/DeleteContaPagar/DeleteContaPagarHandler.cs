using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.ContasPagar.DeleteContaPagar
{
    public class DeleteContaPagarHandler : IRequestHandler<DeleteContaPagarCommand, bool>
    {
        private readonly IContaPagarRepository _repository;

        public DeleteContaPagarHandler(IContaPagarRepository repository) => _repository = repository;

        public async Task<bool> Handle(DeleteContaPagarCommand request, CancellationToken cancellationToken)
        {
            var contaPagar = await _repository.GetByIdAsync(request.Id);
            if (contaPagar is null) return false;

            await _repository.DeleteAsync(contaPagar);
            return true;
        }
    }
}
