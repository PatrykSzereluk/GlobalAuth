using MediatR;
using GlobalAuth.Application.Common;
using GlobalAuth.Application.Features.Users.Dtos;

namespace GlobalAuth.Application.Features.Users.Commands.RefreshToken
{
    public record RefreshTokenCommand(
        Guid UserId,
        Guid AppClientId,
        string RefreshToken,
        string Device,
        string Ip,
        string UserAgent
    ) : IRequest<ApiResponse<AuthResultDto>>;
}
