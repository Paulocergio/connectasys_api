using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.Clientes.GetAllClientes
{
    public class GetAllClientesHandler : IRequestHandler<GetAllClientesQuery, List<ClienteDto>>
    {
        private readonly IClienteRepository _repository;

        public GetAllClientesHandler(IClienteRepository repository) => _repository = repository;

        public async Task<List<ClienteDto>> Handle(GetAllClientesQuery request, CancellationToken cancellationToken)
        {
            var clientes = await _repository.GetAllAsync();

            return clientes.Select(c => new ClienteDto
            {
                Id = c.Id,
                Nome = c.Nome,
                Email = c.Email,
                Telefone = c.Telefone,
                Cpf = c.Cpf,
                Cnpj = c.Cnpj,
                RazaoSocial = c.RazaoSocial,
                Cep = c.Cep,
                Logradouro = c.Logradouro,
                Bairro = c.Bairro,
                Municipio = c.Municipio,
                Uf = c.Uf,
                DataCadastro = c.DataCadastro
            }).ToList();
        }
    }
}
