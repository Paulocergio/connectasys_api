using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.Commands.ContasReceber.CreateContaReceber;
using connectasys_api.Core.Application.Commands.ContasReceber.UpdateContaReceber;
using connectasys_api.Core.Application.Commands.ContasReceber.DeleteContaReceber;
using connectasys_api.Core.Application.Queries.ContasReceber.GetAllContasReceber;
using connectasys_api.Core.Application.Queries.ContasReceber.GetContaReceberById;
using connectasys_api.Core.Application.Queries.ContasReceber.GetContasReceberByClienteId;

namespace connectasys_api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContasReceberController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContasReceberController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _mediator.Send(new GetAllContasReceberQuery()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetContaReceberByIdQuery { Id = id });
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> GetByClienteId(int clienteId) =>
            Ok(await _mediator.Send(new GetContasReceberByClienteIdQuery { ClienteId = clienteId }));

        [HttpPost]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Financeiro}")]
        public async Task<IActionResult> Create(CreateContaReceberCommand command)
        {
            var result = await _mediator.Send(command);
            if (result is null) return BadRequest("ClienteId inválido.");
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Financeiro}")]
        public async Task<IActionResult> Update(int id, UpdateContaReceberCommand command)
        {
            if (id != command.Id) return BadRequest();

            var result = await _mediator.Send(command);
            return result switch
            {
                UpdateContaReceberResult.Success => NoContent(),
                UpdateContaReceberResult.ContaNotFound => NotFound(),
                UpdateContaReceberResult.ClienteInvalido => BadRequest("ClienteId inválido."),
                UpdateContaReceberResult.FormaPagamentoInvalida =>
                    BadRequest("FormaPagamento é obrigatória e deve ser Cartão, Pix, Boleto ou Dinheiro ao informar DataRecebimento."),
                _ => StatusCode(500)
            };
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Financeiro}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteContaReceberCommand { Id = id });
            return success ? NoContent() : NotFound();
        }
    }
}
