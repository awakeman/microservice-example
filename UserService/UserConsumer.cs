using MassTransit;
using Common;

public class UserConsumer : IConsumer<Message>
{
    readonly ILogger logger;

    public UserConsumer(ILogger<UserConsumer> logger)
    {
        this.logger = logger;
    }

    public Task Consume(ConsumeContext<Message> context)
    {
        logger.LogInformation("Received Message for tenant '{Tenant}': '{Body}'", context.Message.Tenant, context.Message.Body);

        // TODO: write to a file in dir named after tenant

        return Task.CompletedTask;
    }
}
