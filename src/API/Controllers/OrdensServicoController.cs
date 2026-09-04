using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using connectasys_api.Core.Application.Commands.OrdensServico.AddItemOrdemServico;
using connectasys_api.Core.Application.Commands.OrdensServico.CreateOrdemServico;
using connectasys_api.Core.Application.Commands.OrdensServico.DeleteOrdemServico;
using connectasys_api.Core.Application.Commands.OrdensServico.RemoveItemOrdemServico;
using connectasys_api.Core.Application.Commands.OrdensServico.UpdateOrdemServico;
using connectasys_api.Core.Application.Queries.OrdensServico.GetAllOrdensServico;
using connectasys_api.Core.Application.Queries.OrdensServico.GetOrdemServicoById;
using connectasys_api.Core.Application.Queries.OrdensServico.GetOrdensServicoByClienteId;
using connectasys_api.Core.Application.Queries.OrdensServico.GetOrdensServicoByVeiculoId;

namespace connectasys_api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdensServicoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdensServicoController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _mediator.Send(new GetAllOrdensServicoQuery()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetOrdemServicoByIdQuery { Id = id });
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> GetByClienteId(int clienteId) =>
            Ok(await _mediator.Send(new GetOrdensServicoByClienteIdQuery { ClienteId = clienteId }));

        [HttpGet("veiculo/{veiculoId}")]
        public async Task<IActionResult> GetByVeiculoId(int veiculoId) =>
            Ok(await _mediator.Send(new GetOrdensServicoByVeiculoIdQuery { VeiculoId = veiculoId }));

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrdemServicoCommand command)
        {
            var result = await _mediator.Send(command);
            if (result is null) return BadRequest("ClienteId, VeiculoId ou TecnicoId inválido.");
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateOrdemServicoCommand command)
        {
            if (id != command.Id) return BadRequest();
            var result = await _mediator.Send(command);
            return result switch
            {
                UpdateOrdemServicoResult.Success => NoContent(),
                UpdateOrdemServicoResult.OrdemServicoNotFound => NotFound(),
                UpdateOrdemServicoResult.ClienteOuVeiculoInvalido => BadRequest("ClienteId ou VeiculoId inválido."),
                UpdateOrdemServicoResult.TecnicoInvalido => BadRequest("TecnicoId inválido."),
                UpdateOrdemServicoResult.StatusInvalido => BadRequest("Status inválido."),
                _ => StatusCode(500)
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteOrdemServicoCommand { Id = id });
            return success ? NoContent() : NotFound();
        }

        [HttpPost("{id}/itens")]
        public async Task<IActionResult> AddItem(int id, AddItemOrdemServicoCommand command)
        {
            if (id != command.OrdemServicoId) return BadRequest();
            var result = await _mediator.Send(command);
            return result is null ? NotFound("Ordem de serviço não encontrada.") : Ok(result);
        }

        [HttpDelete("itens/{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var success = await _mediator.Send(new RemoveItemOrdemServicoCommand { ItemId = itemId });
            return success ? NoContent() : NotFound();
        }
    }
}
