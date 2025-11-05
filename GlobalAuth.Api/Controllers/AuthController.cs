using MediatR;
using GlobalAuth.Api.Common;
using Microsoft.AspNetCore.Mvc;
using GlobalAuth.Application.Abstraction;
using GlobalAuth.Application.Common.Models;
using GlobalAuth.Application.Features.Users.Commands.Logout;
using GlobalAuth.Application.Features.Users.Commands.LogoutAll;
using GlobalAuth.Application.Features.Users.Commands.LoginUser;
using GlobalAuth.Application.Features.Users.Queries.GetSessions;
using GlobalAuth.Application.Features.Users.Commands.RefreshToken;
using GlobalAuth.Application.Features.Users.Commands.RegisterUser;

namespace GlobalAuth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _localizer;

        public AuthController(IMediator mediator, ILocalizationService localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
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

        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions([FromQuery] Guid userId, [FromQuery] Guid appClientId)
        {
            var result = await _mediator.Send(new GetSessionsQuery(userId, appClientId));
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest body)
        {
            await _mediator.Send(new LogoutCommand(body.UserId, body.AppClientId, body.Token));
            return Ok(new { message = _localizer["Success_SingleLogout"] });
        }

        [HttpPost("logout-all-from-app")]
        public async Task<IActionResult> LogoutAllFromApp([FromBody] LogoutAllFromAppRequest body)
        {
            await _mediator.Send(new LogoutAllFromAppCommand(body.UserId, body.AppClientId));
            return Ok(new { message = _localizer["Success_AllLogout"] });
        }

        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll([FromBody] Guid userId)
        {
            await _mediator.Send(new LogoutAllCommand(userId));
            return Ok(new { message = _localizer["Success_AllLogout"] });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest body, [FromServices] IRequestContextService context)
        {
            var command = new RefreshTokenCommand(
                body.UserId,
                body.AppClientId,
                body.RefreshToken,
                context.GetDeviceDescription(),
                context.GetClientIp(),
                context.GetUserAgent());

            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
