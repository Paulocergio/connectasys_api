using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.Clientes.GetClienteById
{
    public class GetClienteByIdQuery : IRequest<ClienteDto?>
    {
        public int Id { get; set; }
    }
}
