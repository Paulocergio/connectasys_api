using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.Usuarios.GetUsuarioById
{
    public class GetUsuarioByIdHandler : IRequestHandler<GetUsuarioByIdQuery, UsuarioDto?>
    {
        private readonly IUsuarioRepository _repository;

        public GetUsuarioByIdHandler(IUsuarioRepository repository) => _repository = repository;

        public async Task<UsuarioDto?> Handle(GetUsuarioByIdQuery request, CancellationToken cancellationToken)
        {
            var usuario = await _repository.GetByIdAsync(request.Id);
            if (usuario is null) return null;

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