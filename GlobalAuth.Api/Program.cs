using Serilog;
using GlobalAuth.Api.Extension;
using GlobalAuth.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();

builder.Services.AddRedis(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services
    .AddSQLite(builder.Configuration)
    .AddLocalizations()
    .AddMediatR()
    .AddJWTOptions(builder.Configuration)
    .AddCustomRateLimiterOptions(builder.Configuration)
    .AddCustomServices();


var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        await AuthDbSeeder.SeedAsync(scope.ServiceProvider);
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseLocalization();

app.RegisterMiddlewares();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();