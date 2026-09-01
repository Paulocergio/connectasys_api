using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.Veiculos.GetAllVeiculos
{
    public class GetAllVeiculosQuery : IRequest<List<VeiculoDto>> { }
}
