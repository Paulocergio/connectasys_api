using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.ContasReceber.DeleteContaReceber
{
    public class DeleteContaReceberHandler : IRequestHandler<DeleteContaReceberCommand, bool>
    {
        private readonly IContaReceberRepository _repository;

        public DeleteContaReceberHandler(IContaReceberRepository repository) => _repository = repository;

        public async Task<bool> Handle(DeleteContaReceberCommand request, CancellationToken cancellationToken)
        {
            var contaReceber = await _repository.GetByIdAsync(request.Id);
            if (contaReceber is null) return false;

            await _repository.DeleteAsync(contaReceber);
            return true;
        }
    }
}
