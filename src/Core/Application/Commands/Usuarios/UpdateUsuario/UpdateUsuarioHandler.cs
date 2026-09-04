using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Application.Interfaces.Services;

namespace connectasys_api.Core.Application.Commands.Usuarios.UpdateUsuario
{
    public class UpdateUsuarioHandler : IRequestHandler<UpdateUsuarioCommand, ResultadoAtualizacaoUsuario>
    {
        private readonly IUsuarioRepository _repository;
        private readonly IPasswordHasher _passwordHasher;

        public UpdateUsuarioHandler(IUsuarioRepository repository, IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResultadoAtualizacaoUsuario> Handle(UpdateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _repository.GetByIdAsync(request.Id);
            if (usuario is null) return ResultadoAtualizacaoUsuario.NaoEncontrado;

            var existente = await _repository.GetByEmailAsync(request.Email);
            if (existente is not null && existente.Id != request.Id) return ResultadoAtualizacaoUsuario.EmailEmUso;

            if (!Roles.EhValida(request.Role)) return ResultadoAtualizacaoUsuario.RoleInvalida;

            usuario.Nome = request.Nome;
            usuario.Email = request.Email;
            usuario.Role = request.Role;
            usuario.Telefone = request.Telefone;

            if (!string.IsNullOrWhiteSpace(request.Senha))
            {
                usuario.SenhaHash = _passwordHasher.Hash(request.Senha);
            }

            await _repository.UpdateAsync(usuario);
            return ResultadoAtualizacaoUsuario.Sucesso;
        }
    }
}