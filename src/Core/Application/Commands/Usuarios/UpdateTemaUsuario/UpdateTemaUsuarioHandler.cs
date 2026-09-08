using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.Usuarios.UpdateTemaUsuario
{
    public class UpdateTemaUsuarioHandler : IRequestHandler<UpdateTemaUsuarioCommand, bool>
    {
        private readonly IUsuarioRepository _repository;

        public UpdateTemaUsuarioHandler(IUsuarioRepository repository) => _repository = repository;

        public async Task<bool> Handle(UpdateTemaUsuarioCommand request, CancellationToken cancellationToken)
        {
            if (!Temas.EhValido(request.Tema)) return false;

            var usuario = await _repository.GetByIdAsync(request.Id);
            if (usuario is null) return false;

            usuario.Tema = request.Tema;
            await _repository.UpdateAsync(usuario);
            return true;
        }
    }
}
