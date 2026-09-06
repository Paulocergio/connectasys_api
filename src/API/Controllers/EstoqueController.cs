using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using connectasys_api.Core.Application.Commands.Estoques.CreateEstoque;
using connectasys_api.Core.Application.Commands.Estoques.UpdateEstoque;
using connectasys_api.Core.Application.Commands.Estoques.DeleteEstoque;
using connectasys_api.Core.Application.Queries.Estoques.GetAllEstoque;
using connectasys_api.Core.Application.Queries.Estoques.GetEstoqueById;

namespace connectasys_api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EstoqueController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EstoqueController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _mediator.Send(new GetAllEstoqueQuery()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetEstoqueByIdQuery { Id = id });
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEstoqueCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateEstoqueCommand command)
        {
            if (id != command.Id) return BadRequest();

            var result = await _mediator.Send(command);
            return result switch
            {
                UpdateEstoqueResult.Success => NoContent(),
                UpdateEstoqueResult.NotFound => NotFound(),
                _ => StatusCode(500)
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteEstoqueCommand { Id = id });
            return success ? NoContent() : NotFound();
        }
    }
}
