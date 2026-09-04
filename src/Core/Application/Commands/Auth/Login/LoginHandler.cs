using MediatR;
using Microsoft.Extensions.Caching.Memory;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Application.Interfaces.Services;

namespace connectasys_api.Core.Application.Commands.Auth.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResultado>
    {
        private const int MaxTentativasFalhas = 5;
        private static readonly TimeSpan JanelaTentativas = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan DuracaoBloqueio = TimeSpan.FromMinutes(15);

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IMemoryCache _cache;

        public LoginHandler(
            IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IMemoryCache cache)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _cache = cache;
        }

        public async Task<LoginResultado> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var emailChave = request.Email.Trim().ToLowerInvariant();
            var chaveBloqueado = $"login-bloqueado:{emailChave}";
            var chaveTentativas = $"login-tentativas:{emailChave}";

            if (_cache.TryGetValue(chaveBloqueado, out _))
            {
                return new LoginResultado { Status = LoginStatus.Bloqueado };
            }

            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuario is null || !_passwordHasher.Verify(request.Senha, usuario.SenhaHash))
            {
                var tentativas = _cache.GetOrCreate(chaveTentativas, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = JanelaTentativas;
                    return 0;
                });
                tentativas++;
                _cache.Set(chaveTentativas, tentativas, JanelaTentativas);

                if (tentativas >= MaxTentativasFalhas)
                {
                    _cache.Set(chaveBloqueado, true, DuracaoBloqueio);
                    _cache.Remove(chaveTentativas);
                    return new LoginResultado { Status = LoginStatus.Bloqueado };
                }

                return new LoginResultado { Status = LoginStatus.Invalido };
            }

            _cache.Remove(chaveTentativas);
            _cache.Remove(chaveBloqueado);

            var (token, expiraEmUtc) = _tokenService.GerarToken(usuario.Id, usuario.Email, usuario.Nome, usuario.Role);

            return new LoginResultado
            {
                Status = LoginStatus.Sucesso,
                Resposta = new LoginResponseDto
                {
                    Token = token,
                    ExpiraEmUtc = expiraEmUtc,
                    UsuarioId = usuario.Id,
                    Nome = usuario.Nome,
                    Role = usuario.Role
                }
            };
        }
    }
}
