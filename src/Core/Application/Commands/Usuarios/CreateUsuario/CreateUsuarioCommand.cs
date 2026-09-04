using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Commands.Usuarios.CreateUsuario
{
    public enum ResultadoCriacaoUsuario
    {
        Sucesso,
        EmailEmUso,
        RoleInvalida
    }

    public class CriarUsuarioResultado
    {
        public ResultadoCriacaoUsuario Resultado { get; set; }
        public UsuarioDto? Usuario { get; set; }
    }

    public class CreateUsuarioCommand : IRequest<CriarUsuarioResultado>
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
