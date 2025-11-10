namespace GlobalAuth.Application.Common
{
    public class RabbitOptions
    {
        public required string HostName { get; init; }
        public required string UserName { get; init; }
        public required string Password { get; init; }
        public required string QueueName { get; init; }
    }
}
