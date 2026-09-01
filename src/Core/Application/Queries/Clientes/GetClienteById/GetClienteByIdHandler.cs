using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.Clientes.GetClienteById
{
    public class GetClienteByIdHandler : IRequestHandler<GetClienteByIdQuery, ClienteDto?>
    {
        private readonly IClienteRepository _repository;

        public GetClienteByIdHandler(IClienteRepository repository) => _repository = repository;

        public async Task<ClienteDto?> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
        {
            var cliente = await _repository.GetByIdAsync(request.Id);
            if (cliente is null) return null;

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
