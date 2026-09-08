using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.Veiculos.GetVeiculoById
{
    public class GetVeiculoByIdHandler : IRequestHandler<GetVeiculoByIdQuery, VeiculoDto?>
    {
        private readonly IVeiculoRepository _repository;

        public GetVeiculoByIdHandler(IVeiculoRepository repository) => _repository = repository;

        public async Task<VeiculoDto?> Handle(GetVeiculoByIdQuery request, CancellationToken cancellationToken)
        {
            var veiculo = await _repository.GetByIdAsync(request.Id);
            if (veiculo is null) return null;

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
