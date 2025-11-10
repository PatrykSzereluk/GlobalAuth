using NotificationService.Infrastructure.Messaging;

namespace NotificationService.Worker
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRabbitMQOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitConfiguration>(configuration.GetSection("RabbitMQ"));

            return services;
        }
    }
}
