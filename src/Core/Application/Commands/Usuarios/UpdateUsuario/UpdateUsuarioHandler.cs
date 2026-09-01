using MediatR;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.Usuarios.UpdateUsuario
{
    public class UpdateUsuarioHandler : IRequestHandler<UpdateUsuarioCommand, bool>
    {
        private readonly IUsuarioRepository _repository;

        public UpdateUsuarioHandler(IUsuarioRepository repository) => _repository = repository;

        public async Task<bool> Handle(UpdateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _repository.GetByIdAsync(request.Id);
            if (usuario is null) return false;

            usuario.Nome = request.Nome;
            usuario.Email = request.Email;
            usuario.Role = request.Role;
            usuario.Telefone = request.Telefone;

            await _repository.UpdateAsync(usuario);
            return true;
        }
    }
}