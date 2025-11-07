using MediatR;
using GlobalAuth.Application.Abstraction;

namespace GlobalAuth.Application.Features.Users.Commands.LogoutAll
{
    public class LogoutAllFromAppHandler : IRequestHandler<LogoutAllFromAppCommand>
    {
        private readonly IRefreshTokenService _refreshService;

        public LogoutAllFromAppHandler(IRefreshTokenService refreshService)
        {
            _refreshService = refreshService;
        }

        public async Task Handle(LogoutAllFromAppCommand request, CancellationToken cancellationToken)
        {
            await _refreshService.RevokeAllForAppAsync(request.UserId, request.AppClientId);
        }
    }
}
