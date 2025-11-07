using MediatR;
using GlobalAuth.Application.Common;
using GlobalAuth.Application.Features.Users.Dtos;

namespace GlobalAuth.Application.Features.Users.Commands.LoginUser
{
    public record LoginUserCommand(string Email, string Password, string ClientId, string Device, string Ip, string UserAgent) : IRequest<ApiResponse<AuthResultDto>>;
}
