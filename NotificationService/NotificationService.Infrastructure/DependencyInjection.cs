using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Infrastructure.Channels;
using NotificationService.Application.Abstractions;
using NotificationService.Infrastructure.Messaging;

namespace NotificationService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IMessageBus, MessageBus>();
            services.AddScoped<INotificationChannel, EmailChannel>();

            return services;
        }


    }

}
