using MediatR;
using Microsoft.AspNetCore.Mvc;
using connectasys_api.Core.Application.Commands.ContasPagar.CreateContaPagar;
using connectasys_api.Core.Application.Commands.ContasPagar.UpdateContaPagar;
using connectasys_api.Core.Application.Commands.ContasPagar.DeleteContaPagar;
using connectasys_api.Core.Application.Queries.ContasPagar.GetAllContasPagar;
using connectasys_api.Core.Application.Queries.ContasPagar.GetContaPagarById;

namespace connectasys_api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContasPagarController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContasPagarController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _mediator.Send(new GetAllContasPagarQuery()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetContaPagarByIdQuery { Id = id });
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateContaPagarCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateContaPagarCommand command)
        {
            if (id != command.Id) return BadRequest();
            var success = await _mediator.Send(command);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteContaPagarCommand { Id = id });
            return success ? NoContent() : NotFound();
        }
    }
}
