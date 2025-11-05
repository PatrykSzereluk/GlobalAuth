using System.Text.Json;
using StackExchange.Redis;
using GlobalAuth.Domain.Tokens;
using System.Security.Cryptography;
using GlobalAuth.Application.Common;
using Microsoft.EntityFrameworkCore;
using GlobalAuth.Infrastructure.Data;
using GlobalAuth.Application.Common.DTOs;
using GlobalAuth.Application.Abstraction;

namespace GlobalAuth.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IDatabase _redis;
        private readonly AuthDbContext _authDbContext;
        private readonly ILocalizationService _localizer;

        public RefreshTokenService(IConnectionMultiplexer connection, AuthDbContext authDbContext, ILocalizationService localizer)
        {
            _redis = connection.GetDatabase();
            _authDbContext = authDbContext;
            _localizer = localizer;
        }

        private static string KeyRefresh(Guid userId, Guid appClientId, string token)
        => $"{RedisKeyNames.Refresh}:{userId}:{appClientId}:{token}";

        private static string KeySessions(Guid userId, Guid appClientId)
            => $"{RedisKeyNames.Sessions}:{userId}:{appClientId}";

        private static string KeyAllToken(Guid userId)
            => $"{RedisKeyNames.AllToken}:{userId}";

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
            await _redis.ListRightPushAsync(KeyAllToken(userId), $"{token}|{appClientId}");
            await _redis.KeyExpireAsync(KeyAllToken(userId), TimeSpan.FromDays(ttlDays));

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

        public async Task<IReadOnlyList<RefreshSessionResponse>> GetActiveSessionsAsync(Guid userId, Guid appClientId)
        {
            var listKey = KeySessions(userId, appClientId);
            var tokens = await _redis.ListRangeAsync(listKey);

            var result = new List<RefreshSessionResponse>(tokens.Length);

            foreach (var t in tokens)
            {
                var key = KeyRefresh(userId, appClientId, t!);
                var json = await _redis.StringGetAsync(key);
                if (!json.HasValue) continue;

                try
                {
                    var rt = JsonSerializer.Deserialize<RefreshToken>(json!);
                    if (rt is null) continue;

                    result.Add(new RefreshSessionResponse(
                        Token: rt.Token,
                        Device: rt.Device,
                        IpAddress: rt.IpAddress,
                        UserAgent: rt.UserAgent,
                        ExpiresAtUtc: rt.ExpiresAtUtc
                    ));
                }
                catch
                {
                }
            }

            return result;
        }

        public async Task RevokeAllAsync(Guid userId, string reason = RefreshTokenRevokeReasons.LogoutAll)
        {
            var listKey = KeyAllToken(userId);
            var values = await _redis.ListRangeAsync(listKey);
            var now = DateTime.UtcNow;

            foreach (var pair in values)
            {
                var parts = pair.ToString().Split('|');
            
                if(parts.Length != 2) continue;

                var token = parts[0];
                var clientIdStr = parts[1];

                if (!Guid.TryParse(clientIdStr, out var appClientId))
                    continue;

                await _redis.KeyDeleteAsync(KeyRefresh(userId, appClientId, token));
                await _redis.ListRemoveAsync(KeySessions(userId, appClientId), token);

                var rows = await _authDbContext.RefreshTokenArchive
                    .Where(r => r.UserId == userId && r.ApplicationClientId == appClientId && r.Token == token && r.RevokedAtUtc == null)
                    .ToListAsync();

                foreach (var row in rows)
                {
                    row.RevokedAtUtc = now;
                    row.RevokedReason = reason;
                }
            }

            await _redis.KeyDeleteAsync(listKey);
            await _authDbContext.SaveChangesAsync();
        }

        public async Task RevokeAllForAppAsync(Guid userId, Guid appClientId, string reason = RefreshTokenRevokeReasons.LogoutAllForApp)
        {
            var listKey = KeySessions(userId, appClientId);
            var tokens = await _redis.ListRangeAsync(listKey);

            foreach (var t in tokens)
            {
                await _redis.KeyDeleteAsync(KeyRefresh(userId, appClientId, t!));
                await _redis.ListRemoveAsync(KeyAllToken(userId), $"{t}|{appClientId}");
            }

            await _redis.KeyDeleteAsync(listKey);

            if(await _redis.KeyRefCountAsync(KeyAllToken(userId)) == 0)
                await _redis.KeyDeleteAsync(KeyAllToken(userId));

            var rows = await _authDbContext.RefreshTokenArchive
                .Where(r => r.UserId == userId && r.ApplicationClientId == appClientId && r.RevokedAtUtc == null)
                .ToListAsync();

            var now = DateTime.UtcNow;
            foreach (var row in rows)
            {
                row.RevokedAtUtc = now;
                row.RevokedReason = reason;
            }
            await _authDbContext.SaveChangesAsync();
        }

        public async Task RevokeAsync(Guid userId, Guid appClientId, string token, string reason = RefreshTokenRevokeReasons.Revoked)
        {
            await _redis.KeyDeleteAsync(KeyRefresh(userId, appClientId, token));
            await _redis.ListRemoveAsync(KeySessions(userId, appClientId), token);
            await _redis.ListRemoveAsync(KeyAllToken(userId), $"{token}|{appClientId}");

            var rows = await _authDbContext.RefreshTokenArchive
                .Where(r => r.UserId == userId && r.ApplicationClientId == appClientId && r.Token == token && r.RevokedAtUtc == null)
                .ToListAsync();

            foreach (var row in rows)
            {
                row.RevokedAtUtc = DateTime.UtcNow;
                row.RevokedReason = reason;
            }
            await _authDbContext.SaveChangesAsync();
        }

        public async Task<string> RotateAsync(Guid userId, Guid appClientId, string oldToken, string device, string ip, string userAgent, int ttlDays = 7)
        {
            var oldKey = KeyRefresh(userId, appClientId, oldToken);
            var existing = await _redis.StringGetAsync(oldKey);
            if (!existing.HasValue)
                throw new InvalidOperationException(_localizer["Error_RefreshTokenDoesntExists"]);

            await _redis.KeyDeleteAsync(oldKey);
            await _redis.ListRemoveAsync(KeySessions(userId, appClientId), oldToken);
            await _redis.ListRemoveAsync(KeyAllToken(userId), $"{oldToken}|{appClientId}");

            var archived = await _authDbContext.RefreshTokenArchive
                .Where(r => r.UserId == userId && r.ApplicationClientId == appClientId && r.Token == oldToken && r.RevokedAtUtc == null)
                .ToListAsync();

            foreach (var row in archived)
            {
                row.RevokedAtUtc = DateTime.UtcNow;
                row.RevokedReason = "rotated";
            }
            await _authDbContext.SaveChangesAsync();

            return await GenerateAsync(userId, appClientId, device, ip, userAgent, ttlDays);

        }

        public async Task<bool> ValidateAsync(Guid userId, Guid appClientId, string token)
        {
            var key = KeyRefresh(userId, appClientId, token);
            var val = await _redis.StringGetAsync(key);
            return val.HasValue;
        }
    }
}
