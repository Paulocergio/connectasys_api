using MediatR;

namespace connectasys_api.Core.Application.Commands.Usuarios.UpdateTemaUsuario
{
    public class UpdateTemaUsuarioCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Tema { get; set; } = string.Empty;
    }
}
