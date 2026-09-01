using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.Usuarios.GetUsuarioById
{
    public class GetUsuarioByIdQuery : IRequest<UsuarioDto?>
    {
        public Guid Id { get; set; }
    }
}