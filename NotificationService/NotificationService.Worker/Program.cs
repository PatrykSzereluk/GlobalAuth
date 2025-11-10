using Serilog;
using NotificationService.Domain;
using NotificationService.Worker;
using NotificationService.Infrastructure;
using NotificationService.Application.Handlers;
using NotificationService.Application.Abstractions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((context, services) =>
    {
        services.AddRabbitMQOptions(context.Configuration);
        services.AddInfrastructure(context.Configuration);
        services.AddScoped<IMessageHandler<NotificationMessage>, NotificationMessageHandler>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<NotificationMessage>>();

bus.Subscribe<NotificationMessage>("notifications_queue", handler.HandleAsync);

bus.Subscribe<string>(
    "notifications_queue",
    async evt =>
    {
        var message = new NotificationMessage
        {
            Channel = "email",
            To = "blablabla",
            Subject = "Verify your account",
            Body = $"Your verification code: {evt}"
        };

        await handler.HandleAsync(message);
    });

await host.RunAsync();
