using MediatR;
using GlobalAuth.Application.Abstraction;

namespace GlobalAuth.Application.Features.Users.Commands.LogoutAll
{
    public class LogoutAllHandler : IRequestHandler<LogoutAllCommand>
    {
        private readonly IRefreshTokenService _refreshService;

        public LogoutAllHandler(IRefreshTokenService refreshService)
        {
            _refreshService = refreshService;
        }
        public async Task Handle(LogoutAllCommand request, CancellationToken cancellationToken)
        {
            await _refreshService.RevokeAllAsync(request.UserId);
        }
    }
}
