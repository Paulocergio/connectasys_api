using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.Commands.ContasPagar.CreateContaPagar;
using connectasys_api.Core.Application.Commands.ContasPagar.UpdateContaPagar;
using connectasys_api.Core.Application.Commands.ContasPagar.DeleteContaPagar;
using connectasys_api.Core.Application.Queries.ContasPagar.GetAllContasPagar;
using connectasys_api.Core.Application.Queries.ContasPagar.GetContaPagarById;

namespace connectasys_api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
        [Authorize(Roles = $"{Roles.Admin},{Roles.Financeiro}")]
        public async Task<IActionResult> Create(CreateContaPagarCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Financeiro}")]
        public async Task<IActionResult> Update(int id, UpdateContaPagarCommand command)
        {
            if (id != command.Id) return BadRequest();

            var result = await _mediator.Send(command);
            return result switch
            {
                UpdateContaPagarResult.Success => NoContent(),
                UpdateContaPagarResult.ContaNotFound => NotFound(),
                UpdateContaPagarResult.FormaPagamentoInvalida =>
                    BadRequest("FormaPagamento é obrigatória e deve ser Cartão, Pix, Boleto ou Dinheiro ao informar DataPagamento."),
                _ => StatusCode(500)
            };
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Financeiro}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteContaPagarCommand { Id = id });
            return success ? NoContent() : NotFound();
        }
    }
}
