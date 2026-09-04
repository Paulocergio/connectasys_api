using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Application.Interfaces.Services;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.Usuarios.CreateUsuario
{
    public class CreateUsuarioHandler : IRequestHandler<CreateUsuarioCommand, CriarUsuarioResultado>
    {
        private readonly IUsuarioRepository _repository;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUsuarioHandler(IUsuarioRepository repository, IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<CriarUsuarioResultado> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
        {
            if (!Roles.EhValida(request.Role))
                return new CriarUsuarioResultado { Resultado = ResultadoCriacaoUsuario.RoleInvalida };

            var existente = await _repository.GetByEmailAsync(request.Email);
            if (existente is not null)
                return new CriarUsuarioResultado { Resultado = ResultadoCriacaoUsuario.EmailEmUso };

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = request.Nome,
                Email = request.Email,
                Role = request.Role,
                Telefone = request.Telefone,
                SenhaHash = _passwordHasher.Hash(request.Senha),
                DataCriacaoUtc = DateTime.UtcNow
            };

            await _repository.AddAsync(usuario);

            return new CriarUsuarioResultado
            {
                Resultado = ResultadoCriacaoUsuario.Sucesso,
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Role = usuario.Role,
                    Telefone = usuario.Telefone,
                    DataCriacaoUtc = usuario.DataCriacaoUtc
                }
            };
        }
    }
}
