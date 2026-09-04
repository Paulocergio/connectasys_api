using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.Clientes.CreateCliente
{
    public class CreateClienteHandler : IRequestHandler<CreateClienteCommand, ClienteDto?>
    {
        private readonly IClienteRepository _repository;

        public CreateClienteHandler(IClienteRepository repository) => _repository = repository;

        public async Task<ClienteDto?> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.Cpf) && await _repository.GetByCpfAsync(request.Cpf) is not null)
                return null;

            if (!string.IsNullOrWhiteSpace(request.Cnpj) && await _repository.GetByCnpjAsync(request.Cnpj) is not null)
                return null;

            var cliente = new Cliente
            {
                Nome = request.Nome,
                Email = request.Email,
                Telefone = request.Telefone,
                Cpf = request.Cpf,
                Cnpj = request.Cnpj,
                RazaoSocial = request.RazaoSocial,
                Cep = request.Cep,
                Logradouro = request.Logradouro,
                Bairro = request.Bairro,
                Municipio = request.Municipio,
                Uf = request.Uf,
                DataCadastro = DateTime.UtcNow
            };

            await _repository.AddAsync(cliente);

            return new ClienteDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Telefone = cliente.Telefone,
                Cpf = cliente.Cpf,
                Cnpj = cliente.Cnpj,
                RazaoSocial = cliente.RazaoSocial,
                Cep = cliente.Cep,
                Logradouro = cliente.Logradouro,
                Bairro = cliente.Bairro,
                Municipio = cliente.Municipio,
                Uf = cliente.Uf,
                DataCadastro = cliente.DataCadastro
            };
        }
    }
}
