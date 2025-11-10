namespace GlobalAuth.Application.Abstraction.Rabbit
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(string exchange, string routingKey, T message);
    }
}
