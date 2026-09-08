using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.Veiculos.UpdateVeiculo
{
    public class UpdateVeiculoHandler : IRequestHandler<UpdateVeiculoCommand, UpdateVeiculoResult>
    {
        private readonly IVeiculoRepository _repository;
        private readonly IClienteRepository _clienteRepository;

        public UpdateVeiculoHandler(IVeiculoRepository repository, IClienteRepository clienteRepository)
        {
            _repository = repository;
            _clienteRepository = clienteRepository;
        }

        public async Task<UpdateVeiculoResult> Handle(UpdateVeiculoCommand request, CancellationToken cancellationToken)
        {
            var veiculo = await _repository.GetByIdAsync(request.Id);
            if (veiculo is null) return UpdateVeiculoResult.VeiculoNotFound;

            var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
            if (cliente is null) return UpdateVeiculoResult.ClienteInvalido;

            if (!TiposVeiculo.EhValido(request.Tipo)) return UpdateVeiculoResult.TipoInvalido;

            veiculo.ClienteId = request.ClienteId;
            veiculo.Placa = request.Placa;
            veiculo.Marca = request.Marca;
            veiculo.Modelo = request.Modelo;
            veiculo.Ano = request.Ano;
            veiculo.Cor = request.Cor;
            veiculo.Tipo = request.Tipo;

            await _repository.UpdateAsync(veiculo);
            return UpdateVeiculoResult.Success;
        }
    }
}
