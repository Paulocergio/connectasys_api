using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.Veiculos.CreateVeiculo
{
    public class CreateVeiculoHandler : IRequestHandler<CreateVeiculoCommand, VeiculoDto?>
    {
        private readonly IVeiculoRepository _repository;
        private readonly IClienteRepository _clienteRepository;

        public CreateVeiculoHandler(IVeiculoRepository repository, IClienteRepository clienteRepository)
        {
            _repository = repository;
            _clienteRepository = clienteRepository;
        }

        public async Task<VeiculoDto?> Handle(CreateVeiculoCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
            if (cliente is null) return null;

            if (!TiposVeiculo.EhValido(request.Tipo)) return null;

            var veiculo = new Veiculo
            {
                ClienteId = request.ClienteId,
                Placa = request.Placa,
                Marca = request.Marca,
                Modelo = request.Modelo,
                Ano = request.Ano,
                Cor = request.Cor,
                Tipo = request.Tipo,
                DataCadastro = DateTime.UtcNow
            };

            await _repository.AddAsync(veiculo);

            return new VeiculoDto
            {
                Id = veiculo.Id,
                ClienteId = veiculo.ClienteId,
                Placa = veiculo.Placa,
                Marca = veiculo.Marca,
                Modelo = veiculo.Modelo,
                Ano = veiculo.Ano,
                Cor = veiculo.Cor,
                Tipo = veiculo.Tipo,
                DataCadastro = veiculo.DataCadastro
            };
        }
    }
}
