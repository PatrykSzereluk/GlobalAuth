namespace NotificationService.Application.Abstractions
{
    public interface IMessageHandler<T>
    {
        Task HandleAsync(T message);
    }
}
