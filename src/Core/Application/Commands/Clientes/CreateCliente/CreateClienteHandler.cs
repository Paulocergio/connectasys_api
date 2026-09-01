using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.Clientes.CreateCliente
{
    public class CreateClienteHandler : IRequestHandler<CreateClienteCommand, ClienteDto>
    {
        private readonly IClienteRepository _repository;

        public CreateClienteHandler(IClienteRepository repository) => _repository = repository;

        public async Task<ClienteDto> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = new Cliente
            {
                Nome = request.Nome,
                Email = request.Email,
                Telefone = request.Telefone,
                DataCadastro = DateTime.UtcNow
            };

            await _repository.AddAsync(cliente);

            return new ClienteDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Telefone = cliente.Telefone,
                DataCadastro = cliente.DataCadastro
            };
        }
    }
}
