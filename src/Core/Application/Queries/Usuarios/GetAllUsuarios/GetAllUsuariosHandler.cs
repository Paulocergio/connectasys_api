using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Queries.Usuarios.GetAllUsuarios
{
    public class GetAllUsuariosHandler : IRequestHandler<GetAllUsuariosQuery, List<UsuarioDto>>
    {
        private readonly IUsuarioRepository _repository;

        public GetAllUsuariosHandler(IUsuarioRepository repository) => _repository = repository;

        public async Task<List<UsuarioDto>> Handle(GetAllUsuariosQuery request, CancellationToken cancellationToken)
        {
            var usuarios = await _repository.GetAllAsync();

            return usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Role = u.Role,
                Telefone = u.Telefone,
                DataCriacaoUtc = u.DataCriacaoUtc
            }).ToList();
        }
    }
}