using GlobalAuth.Application.Abstraction;
using GlobalAuth.Application.Common;
using MediatR;

namespace GlobalAuth.Application.Features.Users.Commands.Logout
{
    public class LogoutHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IRefreshTokenService _refreshService;

        public LogoutHandler(IRefreshTokenService refreshService)
        {
            _refreshService = refreshService;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _refreshService.RevokeAsync(request.UserId, request.AppClientId, request.Token, RefreshTokenRevokeReasons.Logout);
        }
    }
}
