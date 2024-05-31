using System.IO.Abstractions;
using Common;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using SettingsService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(bus =>
{
    bus.AddConsumer<SettingsConsumer>();

    bus.UsingRabbitMq((context, config) =>
    {
        config.Host("rabbit");

        config.ReceiveEndpoint("settings", e =>
        {
            e.ConfigureConsumer<SettingsConsumer>(context);
        });

        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers();

builder.Services
    .AddTransient<IFileSystem, FileSystem>()
    .AddScoped<ISettingsRepository, SettingsFileRepository>();

// Common services
builder.Services
    .AddTenantHandling()
    .AddNotificationClient();


var app = builder.Build();

// Common tenant middleware
app.UseTenantHandling();

app.MapGet("/", () => "Hello from the users service!");

app.MapControllers();

app.Run();
