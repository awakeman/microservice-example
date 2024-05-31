using MassTransit;
using Common;
using UserService;
using System.IO.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(bus =>
{
    bus.AddConsumer<UserConsumer>();

    bus.UsingRabbitMq((context, config) =>
    {
        config.Host("rabbit");

        config.ReceiveEndpoint("users", e =>
        {
            e.ConfigureConsumer<UserConsumer>(context);
        });

        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers();

builder.Services
    .AddTransient<IFileSystem, FileSystem>()
    .AddScoped<IUserRepository, UserFileRepository>()
    .AddTenantHandling()
    .AddNotificationClient();

var app = builder.Build();

app.UseTenantHandling();

app.MapControllers();
app.MapGet("/", () => "Hello from the users service!");

app.Run();
