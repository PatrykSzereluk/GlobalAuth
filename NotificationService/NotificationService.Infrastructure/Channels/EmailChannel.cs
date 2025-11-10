using Serilog;
using NotificationService.Domain;
using NotificationService.Application.Abstractions;

namespace NotificationService.Infrastructure.Channels
{
    public class EmailChannel : INotificationChannel
    {
        public string Name => "email";

        public Task SendAsync(NotificationMessage message)
        {
            Log.Information("Sending EMAIL to {To}: {Subject}\n{Body}", message.To, message.Subject, message.Body);
            return Task.CompletedTask;
        }
    }

}
