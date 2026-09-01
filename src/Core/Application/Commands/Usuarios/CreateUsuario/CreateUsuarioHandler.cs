using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.Usuarios.CreateUsuario
{
    public class CreateUsuarioHandler : IRequestHandler<CreateUsuarioCommand, UsuarioDto>
    {
        private readonly IUsuarioRepository _repository;

        public CreateUsuarioHandler(IUsuarioRepository repository) => _repository = repository;

        public async Task<UsuarioDto> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = request.Nome,
                Email = request.Email,
                Role = request.Role,
                Telefone = request.Telefone,
                DataCriacaoUtc = DateTime.UtcNow
            };

            await _repository.AddAsync(usuario);

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Role = usuario.Role,
                Telefone = usuario.Telefone,
                DataCriacaoUtc = usuario.DataCriacaoUtc
            };
        }
    }
}