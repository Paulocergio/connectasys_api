using MediatR;
using Microsoft.AspNetCore.Mvc;
using connectasys_api.Core.Application.Commands.Usuarios.CreateUsuario;
using connectasys_api.Core.Application.Commands.Usuarios.UpdateUsuario;
using connectasys_api.Core.Application.Commands.Usuarios.DeleteUsuario;
using connectasys_api.Core.Application.Queries.Usuarios.GetAllUsuarios;
using connectasys_api.Core.Application.Queries.Usuarios.GetUsuarioById;

namespace connectasys_api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Create(CreateUsuarioCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateUsuarioCommand command)
        {
            if (id != command.Id) return BadRequest();
            var success = await _mediator.Send(command);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _mediator.Send(new DeleteUsuarioCommand { Id = id });
            return success ? NoContent() : NotFound();
        }
    }
}