using GlobalAuth.Api.Common;
using GlobalAuth.Application.Features.Users.Commands.LoginUser;
using GlobalAuth.Application.Features.Users.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GlobalAuth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest body, [FromServices] IRequestContextService context)
        {
            var command = new LoginUserCommand(
               body.Email,
               body.Password,
               body.ClientId,
               context.GetDeviceDescription(),
               context.GetClientIp(),
               context.GetUserAgent());

            var result = await _mediator.Send(command);
            return Ok(result);
        }


        public record LoginRequest(string Email, string Password, string ClientId);
    }
}
