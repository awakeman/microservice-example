using MassTransit;
using Common;
using System.Text.Json;

namespace UserService;

public class UserConsumer(
    ILogger<UserConsumer> logger,
    IServiceScopeFactory serviceScopeFactory)
    : IConsumer<Message>
{
    readonly ILogger logger = logger;
    readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;

    public async Task Consume(ConsumeContext<Message> context)
    {
        logger.LogInformation("Received Message for tenant '{Tenant}': '{Body}'", context.Message.Tenant, context.Message.Body);

        try
        {
            var model = JsonSerializer.Deserialize<UserModel>(
                context.Message.Body,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;

            if (model == null)
            {
                logger.LogError("Could not deserialise model as a User object");
                return;
            }

            using var scope = serviceScopeFactory.CreateScope();
            var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
            tenantProvider.Tenant = context.Message.Tenant;
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            await repository.SaveUserAsync(model);
        }
        catch (JsonException)
        {
            logger.LogError("Could not deserialise model as a User object");
        }
    }
}
