using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Common;

public static class NotificationExtensions
{
    public static IServiceCollection AddNotificationClient(this IServiceCollection services)
    {
        services.AddSingleton(sp => new HubConnectionBuilder()
                .WithUrl("http://signalr.srv:8080/notifications"));

        return services.AddSingleton<INotificationClient, SignalRNotificationClient>()
            .AddHostedService(sp => (sp.GetRequiredService<INotificationClient>() as SignalRNotificationClient)!);

    }
}
