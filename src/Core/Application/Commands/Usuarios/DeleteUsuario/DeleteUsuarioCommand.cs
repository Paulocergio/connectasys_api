using MediatR;

namespace connectasys_api.Core.Application.Commands.Usuarios.DeleteUsuario
{
    public class DeleteUsuarioCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}