using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Application.Interfaces.Services;

namespace connectasys_api.Core.Application.Commands.Auth.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponseDto?>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public LoginHandler(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuario is null || !_passwordHasher.Verify(request.Senha, usuario.SenhaHash))
            {
                return null;
            }

            var (token, expiraEmUtc) = _tokenService.GerarToken(usuario.Id, usuario.Email, usuario.Nome, usuario.Role);

            return new LoginResponseDto
            {
                Token = token,
                ExpiraEmUtc = expiraEmUtc,
                UsuarioId = usuario.Id,
                Nome = usuario.Nome,
                Role = usuario.Role
            };
        }
    }
}
