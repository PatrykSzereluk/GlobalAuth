using MediatR;
using GlobalAuth.Application.Common.DTOs;

namespace GlobalAuth.Application.Features.Users.Queries.GetSessions
{
    public record GetSessionsQuery(Guid UserId, Guid AppClientId)
        : IRequest<IReadOnlyList<RefreshSessionResponse>>;
}
