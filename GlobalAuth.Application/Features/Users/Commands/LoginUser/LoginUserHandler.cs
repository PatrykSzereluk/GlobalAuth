using MediatR;
using GlobalAuth.Domain.Enums;
using GlobalAuth.Domain.Users;
using GlobalAuth.Application.Common;
using GlobalAuth.Application.Abstraction;
using GlobalAuth.Application.Abstraction.JWT;
using GlobalAuth.Application.Features.Users.Dtos;
using GlobalAuth.Application.Abstraction.Repositories;

namespace GlobalAuth.Application.Features.Users.Commands.LoginUser
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, ApiResponse<AuthResultDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly ILocalizationService _localizer;
        private readonly IRefreshTokenService _refreshTokenService;

        public LoginUserHandler(IUnitOfWork unitOfWork,
            IJwtService jwtService,
            ILocalizationService localizer,
            IRefreshTokenService refreshTokenService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _localizer = localizer;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<ApiResponse<AuthResultDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var appClient = await _unitOfWork.ApplicationClients.FindAsyncFirstOrDefault(c => c.ClientId == request.ClientId);

            if (appClient == null)
                return ApiResponse<AuthResultDto>.Fail(string.Format(_localizer["Error_AppDoesNotExists"], request.ClientId));

            if (appClient.Status != ApplicationClientStatus.Active)
                return ApiResponse<AuthResultDto>.Fail(_localizer["Error_AppInactive"]);

            var normalizedEmail = User.NormalizeEmail(request.Email);

            var user = await _unitOfWork.Users.FindAsyncFirstOrDefault(u => u.NormalizedEmail == normalizedEmail);

            if (user == null)
                return ApiResponse<AuthResultDto>.Fail(_localizer["Error_InvalidCredentials"]);

            if (!user.IsActive)
                return ApiResponse<AuthResultDto>.Fail(_localizer["Error_InactiveUser"]);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return ApiResponse<AuthResultDto>.Fail(_localizer["Error_InvalidCredentials"]);

            var alreadyLinked = await _unitOfWork.UserApplications.FindAsyncFirstOrDefault(
                ua => ua.UserId == user.Id && ua.ApplicationClientId == appClient.Id);

            if (alreadyLinked == null)
            {
                alreadyLinked = new Domain.Applications.UserApplication
                {
                    UserId = user.Id,
                    ApplicationClientId = appClient.Id,
                    IsEnabled = true
                };
                await _unitOfWork.UserApplications.AddAsync(alreadyLinked);
                await _unitOfWork.SaveChangesAsync();

                //return ApiResponse<AuthResultDto>.Fail(_localizer["Error_AccountDoesNotExists"]);
            }

            if (!alreadyLinked.IsEnabled)
                return ApiResponse<AuthResultDto>.Fail(_localizer["Error_BlockedAccount"]);

            var accessToken = _jwtService.GenerateAccessToken(user, appClient.ClientId, appClient.Name);

            var refreshToken = await _refreshTokenService.GenerateAsync(
                user.Id,
                appClient.Id,
                device: request.Device,
                ip: request.Ip,
                userAgent: request.UserAgent);

            var result = new AuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
                User = new UserDto(user.Id, user.Email, user.Role.ToString())
            };

            return ApiResponse<AuthResultDto>.Ok(result);
        }
    }
}
