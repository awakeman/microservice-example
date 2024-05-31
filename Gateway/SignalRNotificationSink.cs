using Microsoft.AspNetCore.SignalR.Client;

public class SignalRNotificationSink : IHostedService
{
    private readonly HubConnection connection;
    private readonly ILogger logger;

    public SignalRNotificationSink(IHubConnectionBuilder hubConnectionBuilder, ILogger<SignalRNotificationSink> logger)
    {
        connection = hubConnectionBuilder.Build();
        this.logger = logger;
        
        connection.Closed += ConnectionClosed;
        connection.On<object>("NotifySaved", OnNotifySaved);
    }

    private void OnNotifySaved(object model)
    {
        logger.LogInformation("Notification received, model saved: {Model}", model);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return connection.StartAsync(cancellationToken);
    }

    private async Task ConnectionClosed(Exception? exception)
    {
        await Task.Delay(2000);
        await connection.StartAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return connection.StopAsync(cancellationToken);
    }
}
