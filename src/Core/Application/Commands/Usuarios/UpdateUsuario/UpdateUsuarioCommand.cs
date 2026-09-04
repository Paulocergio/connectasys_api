using MediatR;

namespace connectasys_api.Core.Application.Commands.Usuarios.UpdateUsuario
{
    public class UpdateUsuarioCommand : IRequest<ResultadoAtualizacaoUsuario>
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string? Senha { get; set; }
    }
}