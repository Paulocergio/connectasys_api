using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Queries.Usuarios.GetAllUsuarios
{
    public class GetAllUsuariosQuery : IRequest<List<UsuarioDto>> { }
}