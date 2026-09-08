using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.Veiculos.GetAllVeiculos
{
    public class GetAllVeiculosHandler : IRequestHandler<GetAllVeiculosQuery, List<VeiculoDto>>
    {
        private readonly IVeiculoRepository _repository;

        public GetAllVeiculosHandler(IVeiculoRepository repository) => _repository = repository;

        public async Task<List<VeiculoDto>> Handle(GetAllVeiculosQuery request, CancellationToken cancellationToken)
        {
            var veiculos = await _repository.GetAllAsync();

            return veiculos.Select(v => new VeiculoDto
            {
                Id = v.Id,
                ClienteId = v.ClienteId,
                Placa = v.Placa,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Ano = v.Ano,
                Cor = v.Cor,
                Tipo = v.Tipo,
                DataCadastro = v.DataCadastro
            }).ToList();
        }
    }
}
