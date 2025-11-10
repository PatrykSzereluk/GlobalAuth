using Serilog;
using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;
using NotificationService.Application.Abstractions;

namespace NotificationService.Infrastructure.Messaging
{
    public class MessageBus: IMessageBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBus(IOptions<RabbitConfiguration> config)
        {
            var factory = new ConnectionFactory
            {
                HostName = config.Value.HostName,
                UserName = config.Value.UserName,
                Password = config.Value.Password,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void Subscribe<T>(string queue, Func<T, Task> handler)
        {
            _channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var message = JsonSerializer.Deserialize<T>(json);
                    if (message != null)
                        await handler(message);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error handling message from {Queue}", queue);
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: true);
                }
            };

            _channel.BasicConsume(queue, false, consumer);
            Log.Information("Subscribed to queue {Queue}", queue);
        }

        public Task PublishAsync<T>(string exchange, string routingKey, T message)
        {
            _channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange, routingKey, basicProperties: null, body);
            Log.Information("Published message {Type} to {RoutingKey}", typeof(T).Name, routingKey);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
