using GlobalAuth.Api.Middlewares;
using Microsoft.Extensions.Options;

namespace GlobalAuth.Api.Extension
{
    public static class WebApplicationExtension
    {
        public static void UseRequestLocalization(this WebApplication app)
        {
            app.UseRequestLocalization(options =>
            {
                options.SetDefaultCulture("en");
                options.AddSupportedCultures("en", "pl");
                options.AddSupportedUICultures("en", "pl");
            });
        }

        public static void UseLocalization(this WebApplication app)
        {
            var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);
        }

        public static void RegisterMiddlewares(this WebApplication app) {
            app.UseMiddleware<RateLimitingMiddleware>();
        }
    }
}
