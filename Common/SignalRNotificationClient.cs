using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;

namespace Common;

public class SignalRNotificationClient : INotificationClient, IHostedService
{
    private readonly HubConnection connection;

    public SignalRNotificationClient(IHubConnectionBuilder hubConnectionBuilder)
    {
        connection = hubConnectionBuilder.Build();
        connection.Closed += ConnectionClosed;
    }

    public Task NotifyAsync<T>(T content)
    {
        return connection.InvokeAsync("ModelSaved", content);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return connection.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return connection.StopAsync(cancellationToken);
    }

    private async Task ConnectionClosed(Exception? exception)
    {
        await Task.Delay(2000);
        await connection.StartAsync();
    }
}