using System.Text.Json;
using StackExchange.Redis;
using GlobalAuth.Domain.Enums;
using GlobalAuth.Domain.Tokens;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using GlobalAuth.Application.Abstraction;
using GlobalAuth.Application.Common.VerificationOptions;

namespace GlobalAuth.Infrastructure.Services
{
    public class VerificationCodeService : IVerificationCodeService
    {
        private readonly IDatabase _redis;
        private readonly VerificationCodeModel _verificationCodeModel;

        public VerificationCodeService(IConnectionMultiplexer connection, IOptions<VerificationCodeOption> codes)
        {
            _redis = connection.GetDatabase();
            _verificationCodeModel = codes.Value.VerificationCodes.Where(t => t.Name == "EmailConfirmation").FirstOrDefault()!;
        }

        private static string Key(Guid userId, UserTokenPurpose purpose, string code)
            => $"verify:{userId}:{purpose}:{code}";

        private static string GenerateNumericCode(int digits)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(digits, 10);

            var max = (int)Math.Pow(10, digits);
            var value = RandomNumberGenerator.GetInt32(0, max);
            return value.ToString($"D{digits}");
        }

        public async Task<string> GenerateAsync(Guid userId, UserTokenPurpose purpose)
        {
            var code = GenerateNumericCode(_verificationCodeModel.DigitLength);
            var entity = new VerificationCode
            {
                Code = code,
                UserId = userId,
                Purpose = purpose,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_verificationCodeModel.TTL)
            };

            var json = JsonSerializer.Serialize(entity);
            var key = Key(userId, purpose, code);

            await _redis.StringSetAsync(key, json, TimeSpan.FromMinutes(_verificationCodeModel.TTL));

            return code;
        }

        public async Task InvalidateAsync(Guid userId, UserTokenPurpose purpose, string code)
        {
            await _redis.KeyDeleteAsync(Key(userId, purpose, code));
        }

        public async Task<bool> ValidateAsync(Guid userId, UserTokenPurpose purpose, string code)
        {
            var key = Key(userId, purpose, code);
            var value = await _redis.StringGetAsync(key);

            if (!value.HasValue)
                return false;

            var data = JsonSerializer.Deserialize<VerificationCode>(value!);
            if (data is null || data.Used || data.ExpiresAtUtc < DateTime.UtcNow)
                return false;

            data.Used = true;
            await _redis.StringSetAsync(key, JsonSerializer.Serialize(data),
                TimeSpan.FromMinutes(10));

            return true;
        }
    }
}
