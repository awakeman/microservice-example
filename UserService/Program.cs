using MassTransit;
using Common;

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

builder.Services
    .AddTenantHandling();

var app = builder.Build();

app.UseTenantHandling();

app.MapGet("/", () => "Hello from the users service!");

app.Run();
