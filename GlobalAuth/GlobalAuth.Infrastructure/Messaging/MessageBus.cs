using Serilog;
using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using Microsoft.Extensions.Options;
using GlobalAuth.Application.Common;
using GlobalAuth.Application.Abstraction.Rabbit;

namespace GlobalAuth.Infrastructure.Messaging
{
    public class MessageBus : IMessageBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName = "notifications";

        public MessageBus(IOptions<RabbitOptions> config)
        {
            var factory = new ConnectionFactory
            {
                HostName = config.Value.HostName,
                UserName = config.Value.UserName,
                Password = config.Value.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, durable: true);
        }

        public Task PublishAsync<T>(string exchange, string routingKey, T message)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange, routingKey, basicProperties: null, body);
            Log.Information("Event published: {Event} → {RoutingKey}", typeof(T).Name, routingKey);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
