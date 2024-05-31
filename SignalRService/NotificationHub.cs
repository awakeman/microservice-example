using Microsoft.AspNetCore.SignalR;

namespace SignalRService;

public class NotificationHub(ILogger<NotificationHub> logger) : Hub
{
    private readonly ILogger logger = logger;
    
    public Task ModelSaved(object model)
    {
        logger.LogInformation("Model Saved, notifying all clients");
        return Clients.All.SendAsync("NotifySaved", model);
    }
}
