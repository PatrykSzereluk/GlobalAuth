using MediatR;

namespace GlobalAuth.Application.Features.Users.Commands.Logout
{
    public record LogoutCommand(Guid UserId, Guid AppClientId, string Token): IRequest;
}
