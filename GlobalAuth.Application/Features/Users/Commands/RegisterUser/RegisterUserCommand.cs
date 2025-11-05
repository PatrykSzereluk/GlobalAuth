using MediatR;
using GlobalAuth.Application.Common;

namespace GlobalAuth.Application.Features.Users.Commands.RegisterUser
{
    public record RegisterUserCommand(string Email, string Password, string ClientId) : IRequest<ApiResponse<string>>;
}
