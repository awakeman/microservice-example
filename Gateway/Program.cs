using Common;
using Gateway;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Ocelot
builder.Configuration.AddJsonFile("ocelot.json");
builder.Services.AddOcelot();

builder.Services.AddMassTransit(bus =>
{
    bus.UsingRabbitMq((context, config) =>
    {
        config.Host("rabbit");
        config.ConfigureEndpoints(context);
    });
});

builder.Services
    .AddTenantHandling()
    .AddScoped<AsyncRequestHandler>();

builder.Services.AddSingleton(sp => new HubConnectionBuilder()
        .WithUrl("http://signalr.srv:8080/notifications"));

builder.Services.AddHostedService<SignalRNotificationSink>();

var app = builder.Build();

app.UseTenantHandling()
   .UseMiddleware<AsyncRequestHandler>();

// Use ocelot routing
await app.UseOcelot();
app.Run();
