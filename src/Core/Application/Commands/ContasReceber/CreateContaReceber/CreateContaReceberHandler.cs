using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.ContasReceber.CreateContaReceber
{
    public class CreateContaReceberHandler : IRequestHandler<CreateContaReceberCommand, ContaReceberDto?>
    {
        private readonly IContaReceberRepository _repository;
        private readonly IClienteRepository _clienteRepository;

        public CreateContaReceberHandler(IContaReceberRepository repository, IClienteRepository clienteRepository)
        {
            _repository = repository;
            _clienteRepository = clienteRepository;
        }

        public async Task<ContaReceberDto?> Handle(CreateContaReceberCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
            if (cliente is null) return null;

            var contaReceber = new ContaReceber
            {
                ClienteId = request.ClienteId,
                Descricao = request.Descricao,
                Valor = request.Valor,
                DataVencimento = DateTime.SpecifyKind(request.DataVencimento, DateTimeKind.Utc),
                DataCadastro = DateTime.UtcNow
            };

            await _repository.AddAsync(contaReceber);

            return new ContaReceberDto
            {
                Id = contaReceber.Id,
                ClienteId = contaReceber.ClienteId,
                Descricao = contaReceber.Descricao,
                Valor = contaReceber.Valor,
                DataVencimento = contaReceber.DataVencimento,
                DataRecebimento = contaReceber.DataRecebimento,
                FormaPagamento = contaReceber.FormaPagamento,
                Status = StatusConta.Calcular(contaReceber.DataRecebimento, contaReceber.DataVencimento),
                DataCadastro = contaReceber.DataCadastro,
                OrdemServicoId = contaReceber.OrdemServicoId
            };
        }
    }
}
