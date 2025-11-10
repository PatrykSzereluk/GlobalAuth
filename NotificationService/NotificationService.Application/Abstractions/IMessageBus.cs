namespace NotificationService.Application.Abstractions
{
    public interface IMessageBus
    {
        void Subscribe<T>(string queue, Func<T, Task> handler);
        Task PublishAsync<T>(string exchange, string routingKey, T message);

    }
}
