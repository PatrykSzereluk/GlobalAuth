using System.Text.Json;
using StackExchange.Redis;
using GlobalAuth.Domain.Tokens;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using GlobalAuth.Infrastructure.Data;
using GlobalAuth.Application.Abstraction;
using GlobalAuth.Application.Common.DTOs;

namespace GlobalAuth.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IDatabase _redis;
        private readonly AuthDbContext _authDbContext;

        public RefreshTokenService(IConnectionMultiplexer connection, AuthDbContext authDbContext)
        {
            _redis = connection.GetDatabase();
            _authDbContext = authDbContext;
        }

        private static string KeyRefresh(Guid userId, Guid appClientId, string token)
        => $"refresh:{userId}:{appClientId}:{token}";

        private static string KeySessions(Guid userId, Guid appClientId)
            => $"sessions:{userId}:{appClientId}";

        private static string GenerateSecureToken(int bytes = 32)
        {
            var buffer = new byte[bytes];
            RandomNumberGenerator.Fill(buffer);
            return Convert.ToBase64String(buffer)
                .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public async Task<string>  GenerateAsync(Guid userId, Guid appClientId, string device, string ip, string userAgent, int ttlDays = 7)
        {
            var token = GenerateSecureToken();
            var expires = DateTime.UtcNow.AddDays(ttlDays);

            var refresh = new RefreshToken
            {
                UserId = userId,
                ApplicationClientId = appClientId,
                Token = token,
                ExpiresAtUtc = expires,
                Device = device,
                IpAddress = ip,
                UserAgent = userAgent
            };

            var json = JsonSerializer.Serialize(refresh);
            var key = KeyRefresh(userId, appClientId, token);

            await _redis.StringSetAsync(key, json, TimeSpan.FromDays(ttlDays));
            await _redis.ListRightPushAsync(KeySessions(userId, appClientId), token);
            await _redis.KeyExpireAsync(KeySessions(userId, appClientId), TimeSpan.FromDays(ttlDays));

            _authDbContext.RefreshTokenArchive.Add(new RefreshToken
            {
                UserId = userId,
                ApplicationClientId = appClientId,
                Token = token,
                ExpiresAtUtc = expires,
                Device = device,
                IpAddress = ip,
                UserAgent = userAgent
            });

            await _authDbContext.SaveChangesAsync();

            return token;
        }

    }
}
