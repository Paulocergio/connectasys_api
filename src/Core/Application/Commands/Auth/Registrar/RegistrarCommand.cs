using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Commands.Auth.Registrar
{
    public enum RegistrarResultado
    {
        Sucesso,
        EmailEmUso
    }

    public class RegistrarResult
    {
        public RegistrarResultado Resultado { get; set; }
        public LoginResponseDto? Resposta { get; set; }
    }

    // Cadastro self-service de uma oficina nova: cria a Empresa (tenant) e o
    // primeiro usuário (Admin) num só passo, com trial de 3 dias, e já
    // devolve um token de login (auto-login, sem passo extra).
    public class RegistrarCommand : IRequest<RegistrarResult>
    {
        public string NomeEmpresa { get; set; } = string.Empty;
        public string NomeUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
