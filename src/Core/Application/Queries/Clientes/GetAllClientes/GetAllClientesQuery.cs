using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.Clientes.GetAllClientes
{
    public class GetAllClientesQuery : IRequest<List<ClienteDto>> { }
}
