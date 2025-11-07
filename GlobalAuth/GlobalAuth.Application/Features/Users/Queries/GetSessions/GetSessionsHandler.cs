using MediatR;
using GlobalAuth.Application.Common.DTOs;
using GlobalAuth.Application.Abstraction;

namespace GlobalAuth.Application.Features.Users.Queries.GetSessions
{
    public class GetSessionsHandler : IRequestHandler<GetSessionsQuery, IReadOnlyList<RefreshSessionResponse>>
    {
        private readonly IRefreshTokenService _refreshService;

        public GetSessionsHandler(IRefreshTokenService refreshService)
        {
            _refreshService = refreshService;
        }

        public async Task<IReadOnlyList<RefreshSessionResponse>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
        {
            var sessions = await _refreshService.GetActiveSessionsAsync(request.UserId, request.AppClientId);
            return sessions;
        }
    }
}
