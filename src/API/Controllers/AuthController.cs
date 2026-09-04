using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var resultado = await _mediator.Send(command);
            return resultado.Status switch
            {
                LoginStatus.Sucesso => Ok(resultado.Resposta),
                LoginStatus.Invalido => Unauthorized(new { message = "Email ou senha inválidos" }),
                LoginStatus.Bloqueado => StatusCode(
                    StatusCodes.Status429TooManyRequests,
                    new { message = "Muitas tentativas com essa conta. Tente novamente em alguns minutos." }),
                _ => StatusCode(500)
            };
        }
    }
}
