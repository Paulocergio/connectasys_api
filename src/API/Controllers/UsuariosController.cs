using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using connectasys_api.Core.Application.Commands.Usuarios.CreateUsuario;
using connectasys_api.Core.Application.Commands.Usuarios.UpdateUsuario;
using connectasys_api.Core.Application.Commands.Usuarios.DeleteUsuario;
using connectasys_api.Core.Application.Queries.Usuarios.GetAllUsuarios;
using connectasys_api.Core.Application.Queries.Usuarios.GetUsuarioById;
using connectasys_api.Core.Application.Common;

namespace connectasys_api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsuariosController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _mediator.Send(new GetAllUsuariosQuery()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetUsuarioByIdQuery { Id = id });
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(CreateUsuarioCommand command)
        {
            var resultado = await _mediator.Send(command);
            return resultado.Resultado switch
            {
                ResultadoCriacaoUsuario.Sucesso =>
                    CreatedAtAction(nameof(GetById), new { id = resultado.Usuario!.Id }, resultado.Usuario),
                ResultadoCriacaoUsuario.EmailEmUso =>
                    Conflict(new { message = "Já existe um usuário cadastrado com este e-mail." }),
                ResultadoCriacaoUsuario.RoleInvalida =>
                    BadRequest(new { message = "Role inválida. Use Admin, Mecânico, Recepcionista ou Financeiro." }),
                _ => throw new InvalidOperationException($"Resultado inesperado: {resultado.Resultado}")
            };
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(Guid id, UpdateUsuarioCommand command)
        {
            if (id != command.Id) return BadRequest();
            var resultado = await _mediator.Send(command);
            return resultado switch
            {
                ResultadoAtualizacaoUsuario.Sucesso => NoContent(),
                ResultadoAtualizacaoUsuario.NaoEncontrado => NotFound(),
                ResultadoAtualizacaoUsuario.EmailEmUso => Conflict(new { message = "Já existe um usuário cadastrado com este e-mail." }),
                ResultadoAtualizacaoUsuario.RoleInvalida =>
                    BadRequest(new { message = "Role inválida. Use Admin, Mecânico, Recepcionista ou Financeiro." }),
                _ => throw new InvalidOperationException($"Resultado inesperado: {resultado}")
            };
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _mediator.Send(new DeleteUsuarioCommand { Id = id });
            return success ? NoContent() : NotFound();
        }
    }
}