using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.Commands.Agendamentos.CreateAgendamento;
using connectasys_api.Core.Application.Commands.Agendamentos.DeleteAgendamento;
using connectasys_api.Core.Application.Commands.Agendamentos.UpdateAgendamento;
using connectasys_api.Core.Application.Queries.Agendamentos.GetAgendamentoById;
using connectasys_api.Core.Application.Queries.Agendamentos.GetAgendamentosByTecnicoId;
using connectasys_api.Core.Application.Queries.Agendamentos.GetAllAgendamentos;
using connectasys_api.Core.Application.Queries.Agendamentos.GetConflitoAgendamento;

namespace connectasys_api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AgendamentosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AgendamentosController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _mediator.Send(new GetAllAgendamentosQuery()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetAgendamentoByIdQuery { Id = id });
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("tecnico/{tecnicoId}")]
        public async Task<IActionResult> GetByTecnicoId(Guid tecnicoId, [FromQuery] DateTime? data) =>
            Ok(await _mediator.Send(new GetAgendamentosByTecnicoIdQuery { TecnicoId = tecnicoId, Data = data }));

        [HttpGet("conflito")]
        public async Task<IActionResult> GetConflito([FromQuery] Guid tecnicoId, [FromQuery] DateTime dataHora)
        {
            var conflito = await _mediator.Send(new GetConflitoAgendamentoQuery { TecnicoId = tecnicoId, DataHora = dataHora });
            return conflito is null ? NoContent() : Ok(conflito);
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Mecanico},{Roles.Recepcionista}")]
        public async Task<IActionResult> Create(CreateAgendamentoCommand command)
        {
            var resultado = await _mediator.Send(command);
            return resultado.Resultado switch
            {
                CriarAgendamentoResultado.Sucesso =>
                    CreatedAtAction(nameof(GetById), new { id = resultado.Agendamento!.Id }, resultado.Agendamento),
                CriarAgendamentoResultado.TecnicoInvalido =>
                    BadRequest(new { message = "Técnico inválido: usuário não encontrado ou não é Mecânico." }),
                CriarAgendamentoResultado.Conflito =>
                    Conflict(new { message = "Técnico já tem agendamento neste horário.", agendamento = resultado.Conflitante }),
                _ => StatusCode(500)
            };
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Mecanico},{Roles.Recepcionista}")]
        public async Task<IActionResult> Update(int id, UpdateAgendamentoCommand command)
        {
            if (id != command.Id) return BadRequest();

            var resultado = await _mediator.Send(command);
            return resultado.Resultado switch
            {
                AtualizarAgendamentoResultado.Sucesso => NoContent(),
                AtualizarAgendamentoResultado.NotFound => NotFound(),
                AtualizarAgendamentoResultado.TecnicoInvalido =>
                    BadRequest(new { message = "Técnico inválido: usuário não encontrado ou não é Mecânico." }),
                AtualizarAgendamentoResultado.StatusInvalido =>
                    BadRequest(new { message = "Status inválido." }),
                AtualizarAgendamentoResultado.Conflito =>
                    Conflict(new { message = "Técnico já tem agendamento neste horário.", agendamento = resultado.Conflitante }),
                _ => StatusCode(500)
            };
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Mecanico},{Roles.Recepcionista}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteAgendamentoCommand { Id = id });
            return success ? NoContent() : NotFound();
        }
    }
}
