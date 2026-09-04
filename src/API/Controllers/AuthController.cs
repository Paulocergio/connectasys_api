using MediatR;
using Microsoft.AspNetCore.Mvc;
using connectasys_api.Core.Application.Commands.Auth.Login;

namespace connectasys_api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return result is null ? Unauthorized(new { message = "Email ou senha inválidos" }) : Ok(result);
        }
    }
}
