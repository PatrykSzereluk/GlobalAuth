using MediatR;
using GlobalAuth.Domain.Enums;
using GlobalAuth.Domain.Users;
using GlobalAuth.Application.Common;
using GlobalAuth.Domain.Applications;
using GlobalAuth.Application.Abstraction;
using GlobalAuth.Application.Abstraction.Repositories;

namespace GlobalAuth.Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizer;

        public RegisterUserHandler(IUnitOfWork unitOfWork, ILocalizationService localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<ApiResponse<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var appClient = await _unitOfWork.ApplicationClients.FindAsyncFirstOrDefault(c => c.ClientId == request.ClientId);

            if (appClient == null)
                return ApiResponse<string>.Fail(string.Format(_localizer["Error_AppDoesNotExists"], request.ClientId));

            if(appClient.Status != ApplicationClientStatus.Active)
                return ApiResponse<string>.Fail(_localizer["Error_AppInactive"]);

            var normalizedEmail = User.NormalizeEmail(request.Email);

            var user = await _unitOfWork.Users.FindAsyncFirstOrDefault(u => u.NormalizedEmail == normalizedEmail);

            if (user == null)
            {
                user = new User
                {
                    Email = request.Email.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    IsActive = true,
                    IsEmailConfirmed = false
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                if (!user.IsActive)
                    return ApiResponse<string>.Fail(_localizer["Error_InactiveUser"]);

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return ApiResponse<string>.Fail(_localizer["Error_UserEmailExsists"]);
            }

            var alreadyLinked =
                    await _unitOfWork.UserApplications.FindAsyncFirstOrDefault(
                        ua => ua.UserId == user.Id && ua.ApplicationClientId == appClient.Id);

            if (alreadyLinked != null)
            {
                if (!alreadyLinked.IsEnabled)
                    return ApiResponse<string>.Fail(_localizer["Error_BlockedAccount"]);
                
                return ApiResponse<string>.Fail(_localizer["Error_AccountExists"]);
            }

            await _unitOfWork.UserApplications.AddAsync(new UserApplication
            {
                UserId = user.Id,
                ApplicationClientId = appClient.Id,
                IsEnabled = true
            });

            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.Ok("User registered successfully.");
        }
    }
}
