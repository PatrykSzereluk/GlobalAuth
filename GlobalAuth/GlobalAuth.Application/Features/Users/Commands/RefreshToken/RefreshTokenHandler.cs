using MediatR;
using GlobalAuth.Application.Common;
using GlobalAuth.Application.Abstraction;
using GlobalAuth.Application.Abstraction.JWT;
using GlobalAuth.Application.Features.Users.Dtos;
using GlobalAuth.Application.Abstraction.Repositories;

namespace GlobalAuth.Application.Features.Users.Commands.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResultDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly ILocalizationService _localizer;
        private readonly IRefreshTokenService _refreshService;

        public RefreshTokenHandler(IUnitOfWork unitOfWork, IJwtService jwtService, IRefreshTokenService refreshService, ILocalizationService localizer)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _refreshService = refreshService;
            _localizer = localizer;
        }

        public async Task<ApiResponse<AuthResultDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var isValid = await _refreshService.ValidateAsync(request.UserId, request.AppClientId, request.RefreshToken);

            if (!isValid)
                throw new UnauthorizedAccessException(_localizer["Error_RefreshTokenDoesntExists"]);

            var user = (await _unitOfWork.Users.FindAsync(u => u.Id == request.UserId)).FirstOrDefault()
                ?? throw new UnauthorizedAccessException(_localizer["Error_UserDoesntExists"]);

            var appClient = (await _unitOfWork.ApplicationClients.FindAsync(c => c.Id == request.AppClientId)).FirstOrDefault()
                ?? throw new UnauthorizedAccessException(_localizer["Error_AppDoesntExists2"]);

            var newRefreshToken = await _refreshService.RotateAsync(
                user.Id,
                appClient.Id,
                request.RefreshToken,
                request.Device,
                request.Ip,
                request.UserAgent
            );

            var newAccessToken = _jwtService.GenerateAccessToken(user, appClient.Id, appClient.Name);

            var result = new AuthResultDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
                User = new UserDto(user.Id, user.Email, user.Role.ToString())
            };

            return ApiResponse<AuthResultDto>.Ok(result);
        }
    }
}
