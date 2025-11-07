using GlobalAuth.Api.Common;
using GlobalAuth.Application.Abstraction;
using GlobalAuth.Application.Abstraction.JWT;
using GlobalAuth.Application.Abstraction.Repositories;
using GlobalAuth.Application.Common;
using GlobalAuth.Application.Common.RateLimiter;
using GlobalAuth.Application.Common.VerificationOptions;
using GlobalAuth.Infrastructure.Data;
using GlobalAuth.Infrastructure.Data.Repositories;
using GlobalAuth.Infrastructure.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;
using System.Globalization;

namespace GlobalAuth.Api.Extension
{
    public static class WebApplicationBuilderExtension
    {
        public static void AddSerilog(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("App", "GlobalAuth")
                .CreateLogger();

            builder.Host.UseSerilog();
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddScoped<ICustomRateLimiter, CustomRateLimiter>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IRequestContextService, RequestContextService>();
            services.AddScoped<IVerificationCodeService, VerificationCodeService>();

            return services;
        }

        public static IServiceCollection AddJWTOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

            return services;
        }

        public static IServiceCollection AddCustomRateLimiterOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RateLimitOptions>(configuration.GetSection("RateLimiter"));

            return services;
        }

        public static IServiceCollection AddVerificationCodesOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<VerificationCodeOption>(configuration.GetSection("VerificationCodes"));

            return services;
        }

        public static IServiceCollection AddLocalizations(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("pl")
            };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new AcceptLanguageHeaderRequestCultureProvider(),
                    new QueryStringRequestCultureProvider(),
                };
            });

            return services;
        }

        public static IServiceCollection AddSQLite(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlite(connectionString));

            return services;
        }

        public static IServiceCollection AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));

            return services;
        }
        
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnection = configuration.GetSection("Redis:ConnectionString").Value;
            
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(redisConnection!));

            return services;
        }
    }
}
