using MassTransit;
using Common;
using SettingsService;
using System.Text.Json;

public class SettingsConsumer : IConsumer<Message>
{
    readonly ILogger logger;
    readonly IServiceScopeFactory serviceScopeFactory;

    public SettingsConsumer (ILogger<SettingsConsumer> logger, IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Consume(ConsumeContext<Message> context)
    {
        logger.LogInformation("Received Message for tenant '{Tenant}': '{Body}'", context.Message.Tenant, context.Message.Body);
        
        var model = JsonSerializer.Deserialize<SettingsModel>(context.Message.Body);
        if (model == null)
        {
            logger.LogError("Could not deserialise model as a Setings object");
            return;
        }

        using var scope = serviceScopeFactory.CreateAsyncScope();
        var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
        tenantProvider.Tenant = context.Message.Tenant;
        var repository = scope.ServiceProvider.GetRequiredService<ISettingsRepository>();
        
        await repository.SaveSettingsAsync(model);
    }
}