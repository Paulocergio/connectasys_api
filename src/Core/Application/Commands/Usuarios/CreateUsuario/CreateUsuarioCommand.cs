using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Commands.Usuarios.CreateUsuario
{
    public class CreateUsuarioCommand : IRequest<UsuarioDto>
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
    }
}