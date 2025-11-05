namespace GlobalAuth.Application.Common
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string SigningKey { get; set; } = default!;
        public int ExpirationMinutes { get; set; } = 15;
    }
}
