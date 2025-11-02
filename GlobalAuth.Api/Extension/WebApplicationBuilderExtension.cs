using Serilog;

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
    }
}
