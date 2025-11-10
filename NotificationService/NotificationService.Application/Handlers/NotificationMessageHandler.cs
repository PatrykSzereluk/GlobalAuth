using Serilog;
using NotificationService.Domain;
using NotificationService.Application.Abstractions;

namespace NotificationService.Application.Handlers
{
    public class NotificationMessageHandler : IMessageHandler<NotificationMessage>
    {
        private readonly IEnumerable<INotificationChannel> _channels;

        public NotificationMessageHandler(IEnumerable<INotificationChannel> channels)
        {
            _channels = channels;
        }

        public async Task HandleAsync(NotificationMessage message)
        {
            var channel = _channels.FirstOrDefault(c => c.Name == message.Channel);
            if (channel == null)
            {
                Log.Warning("Unknown channel: {Channel}", message.Channel);
                return;
            }

            await channel.SendAsync(message);
            Log.Information("Sent notification via {Channel}", message.Channel);
        }
    }
}
