using MassTransit;
using Common;
using SettingsService;
using System.Text.Json;

public class SettingsConsumer(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<SettingsConsumer> logger)
    : IConsumer<Message>
{
    readonly ILogger logger = logger;
    readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;

    public async Task Consume(ConsumeContext<Message> context)
    {
        logger.LogInformation("Received Message for tenant '{Tenant}': '{Body}'", context.Message.Tenant, context.Message.Body);
        
        try
        {
            var model = JsonSerializer.Deserialize<SettingsModel>(
                context.Message.Body,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;

            if(model == null)
            {
                logger.LogError("Could not deserialise model as a Setings object");
                return;
            }

            using var scope = serviceScopeFactory.CreateScope();
            var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
            tenantProvider.Tenant = context.Message.Tenant;
            var repository = scope.ServiceProvider.GetRequiredService<ISettingsRepository>();

            await repository.SaveSettingsAsync(model);
        }
        catch (JsonException)
        {
            logger.LogError("Could not deserialise model as a Setings object");
        }
    }
}