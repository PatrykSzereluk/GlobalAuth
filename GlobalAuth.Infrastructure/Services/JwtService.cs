using System.Text;
using System.Security.Claims;
using GlobalAuth.Domain.Users;
using Microsoft.Extensions.Options;
using GlobalAuth.Application.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using GlobalAuth.Application.Abstraction.JWT;

namespace GlobalAuth.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _options;

        public JwtService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateAccessToken(User user, string? clientId = null, string? applicationName = null)
        {
            var secretKey = _options.SigningKey;
            var issuer = _options.Issuer;
            var audience = _options.Audience;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString()),
                new(ClaimsTypes.ClientId, clientId ?? Const.Unknown),
                new(ClaimsTypes.AplicationName, applicationName ?? Const.Unknown)
            };

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
