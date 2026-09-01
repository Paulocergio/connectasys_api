using MediatR;
using Microsoft.AspNetCore.Mvc;
using connectasys_api.Core.Application.Commands.Clientes.CreateCliente;
using connectasys_api.Core.Application.Commands.Clientes.UpdateCliente;
using connectasys_api.Core.Application.Commands.Clientes.DeleteCliente;
using connectasys_api.Core.Application.Queries.Clientes.GetAllClientes;
using connectasys_api.Core.Application.Queries.Clientes.GetClienteById;

namespace connectasys_api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _mediator.Send(new GetAllClientesQuery()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetClienteByIdQuery { Id = id });
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClienteCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateClienteCommand command)
        {
            if (id != command.Id) return BadRequest();
            var success = await _mediator.Send(command);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteClienteCommand { Id = id });
            return success ? NoContent() : NotFound();
        }
    }
}
