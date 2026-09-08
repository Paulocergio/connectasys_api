using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using connectasys_api.Core.Application.Commands.Veiculos.CreateVeiculo;
using connectasys_api.Core.Application.Commands.Veiculos.UpdateVeiculo;
using connectasys_api.Core.Application.Commands.Veiculos.DeleteVeiculo;
using connectasys_api.Core.Application.Queries.Veiculos.GetAllVeiculos;
using connectasys_api.Core.Application.Queries.Veiculos.GetVeiculoById;
using connectasys_api.Core.Application.Queries.Veiculos.GetVeiculosByClienteId;

namespace connectasys_api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VeiculosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VeiculosController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _mediator.Send(new GetAllVeiculosQuery()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetVeiculoByIdQuery { Id = id });
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> GetByClienteId(int clienteId) =>
            Ok(await _mediator.Send(new GetVeiculosByClienteIdQuery { ClienteId = clienteId }));

        [HttpPost]
        public async Task<IActionResult> Create(CreateVeiculoCommand command)
        {
            var result = await _mediator.Send(command);
            if (result is null) return BadRequest("ClienteId ou tipo inválido.");
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateVeiculoCommand command)
        {
            if (id != command.Id) return BadRequest();

            var result = await _mediator.Send(command);
            return result switch
            {
                UpdateVeiculoResult.Success => NoContent(),
                UpdateVeiculoResult.VeiculoNotFound => NotFound(),
                UpdateVeiculoResult.ClienteInvalido => BadRequest("ClienteId inválido."),
                UpdateVeiculoResult.TipoInvalido => BadRequest("Tipo inválido. Use Carro, Moto, Caminhão ou Outros."),
                _ => StatusCode(500)
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteVeiculoCommand { Id = id });
            return success ? NoContent() : NotFound();
        }
    }
}
