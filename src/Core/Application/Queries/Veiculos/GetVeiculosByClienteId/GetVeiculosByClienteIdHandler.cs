using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.Veiculos.GetVeiculosByClienteId
{
    public class GetVeiculosByClienteIdHandler : IRequestHandler<GetVeiculosByClienteIdQuery, List<VeiculoDto>>
    {
        private readonly IVeiculoRepository _repository;

        public GetVeiculosByClienteIdHandler(IVeiculoRepository repository) => _repository = repository;

        public async Task<List<VeiculoDto>> Handle(GetVeiculosByClienteIdQuery request, CancellationToken cancellationToken)
        {
            var veiculos = await _repository.GetByClienteIdAsync(request.ClienteId);

            return veiculos.Select(v => new VeiculoDto
            {
                Id = v.Id,
                ClienteId = v.ClienteId,
                Placa = v.Placa,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Ano = v.Ano,
                Cor = v.Cor,
                DataCadastro = v.DataCadastro
            }).ToList();
        }
    }
}
