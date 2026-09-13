using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Commands.Auth.Login
{
    public enum LoginStatus
    {
        Sucesso,
        Invalido,
        Bloqueado,
        TesteExpirado
    }

    public class LoginResultado
    {
        public LoginStatus Status { get; set; }
        public LoginResponseDto? Resposta { get; set; }
    }

    public class LoginCommand : IRequest<LoginResultado>
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
