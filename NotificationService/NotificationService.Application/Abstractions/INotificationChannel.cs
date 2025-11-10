using NotificationService.Domain;

namespace NotificationService.Application.Abstractions
{
    public interface INotificationChannel
    {
        string Name { get; }
        Task SendAsync(NotificationMessage message);
    }
}
