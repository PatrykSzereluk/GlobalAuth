using MediatR;

namespace GlobalAuth.Application.Features.Users.Commands.LogoutAll
{
    public record LogoutAllCommand(Guid UserId): IRequest;
}
