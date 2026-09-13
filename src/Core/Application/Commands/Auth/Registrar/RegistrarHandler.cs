using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Application.Interfaces.Services;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.Auth.Registrar
{
    public class RegistrarHandler : IRequestHandler<RegistrarCommand, RegistrarResult>
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public RegistrarHandler(
            IEmpresaRepository empresaRepository,
            IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _empresaRepository = empresaRepository;
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<RegistrarResult> Handle(RegistrarCommand request, CancellationToken cancellationToken)
        {
            var existente = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (existente is not null)
                return new RegistrarResult { Resultado = RegistrarResultado.EmailEmUso };

            var agora = DateTime.UtcNow;

            var empresa = new Empresa
            {
                Id = Guid.NewGuid(),
                Nome = request.NomeEmpresa,
                DataCadastro = agora,
                TrialExpiraEm = Trial.CalcularExpiracao(agora)
            };

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresa.Id,
                Nome = request.NomeUsuario,
                Email = request.Email,
                Role = Roles.Admin,
                Telefone = request.Telefone,
                SenhaHash = _passwordHasher.Hash(request.Senha),
                DataCriacaoUtc = agora
            };

            await _empresaRepository.CriarComPrimeiroUsuarioAsync(empresa, usuario);

            var (token, expiraEmUtc) = _tokenService.GerarToken(
                usuario.Id, usuario.EmpresaId, usuario.Email, usuario.Nome, usuario.Role);

            return new RegistrarResult
            {
                Resultado = RegistrarResultado.Sucesso,
                Resposta = new LoginResponseDto
                {
                    Token = token,
                    ExpiraEmUtc = expiraEmUtc,
                    UsuarioId = usuario.Id,
                    Nome = usuario.Nome,
                    Role = usuario.Role,
                    Tema = usuario.Tema,
                    TrialExpiraEmUtc = empresa.TrialExpiraEm
                }
            };
        }
    }
}
