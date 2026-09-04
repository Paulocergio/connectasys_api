using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.Clientes.UpdateCliente
{
    public class UpdateClienteHandler : IRequestHandler<UpdateClienteCommand, UpdateClienteResult>
    {
        private readonly IClienteRepository _repository;

        public UpdateClienteHandler(IClienteRepository repository) => _repository = repository;

        public async Task<UpdateClienteResult> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _repository.GetByIdAsync(request.Id);
            if (cliente is null) return UpdateClienteResult.ClienteNotFound;

            if (!string.IsNullOrWhiteSpace(request.Cpf))
            {
                var existente = await _repository.GetByCpfAsync(request.Cpf);
                if (existente is not null && existente.Id != request.Id) return UpdateClienteResult.DocumentoEmUso;
            }

            if (!string.IsNullOrWhiteSpace(request.Cnpj))
            {
                var existente = await _repository.GetByCnpjAsync(request.Cnpj);
                if (existente is not null && existente.Id != request.Id) return UpdateClienteResult.DocumentoEmUso;
            }

            cliente.Nome = request.Nome;
            cliente.Email = request.Email;
            cliente.Telefone = request.Telefone;
            cliente.Cpf = request.Cpf;
            cliente.Cnpj = request.Cnpj;
            cliente.RazaoSocial = request.RazaoSocial;
            cliente.Cep = request.Cep;
            cliente.Logradouro = request.Logradouro;
            cliente.Bairro = request.Bairro;
            cliente.Municipio = request.Municipio;
            cliente.Uf = request.Uf;

            await _repository.UpdateAsync(cliente);
            return UpdateClienteResult.Success;
        }
    }
}
