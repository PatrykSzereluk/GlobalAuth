using MediatR;

namespace GlobalAuth.Application.Features.Users.Commands.LogoutAll
{
    public record LogoutAllFromAppCommand(Guid UserId, Guid AppClientId): IRequest;
}
