namespace GlobalAuth.Application.Features.Users.Dtos
{
    public class AuthResultDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTime ExpiresAtUtc { get; set; }
        public UserDto User { get; set; } = default!;
    }
}
