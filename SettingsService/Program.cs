using System.IO.Abstractions;
using Common;
using MassTransit;
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
    .AddTenantHandling()
    .AddScoped<ISettingsRepository, SettingsFileRepository>();

var app = builder.Build();

app.UseTenantHandling();

app.MapGet("/", () => "Hello from the users service!");

app.MapControllers();

app.Run();
