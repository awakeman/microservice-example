
using System.Net;
using Common;
using MassTransit;
using Ocelot.Values;

namespace Gateway;

public class AsyncRequestHandler(
    IBus bus,
    IReadOnlyTenantProvider tenantProvider,
    ILogger<AsyncRequestHandler> logger)
    : IMiddleware
{
    private readonly IReadOnlyTenantProvider tenantProvider = tenantProvider;
    private readonly IBus bus = bus;
    private readonly ILogger logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!(context.Request.Method == "POST"
                && context.Request.Headers.ContainsKey("X-Async")))
        {
            // continue handling request
            await next(context);
            return;
        }

        var path = context.Request.Path;
        
        // TODO: figure out if we can use ocelot to tell us routing info for the type
        //       and name the queue after that somehow
        string queue;
        if (path.StartsWithSegments("/users"))
        {
            queue = "users";
        }
        else if (path.StartsWithSegments("/settings"))
        {
            queue = "settings";
        }
        else
        {
            // continue handling request, probably resulting in an error code
            await next(context);
            return;
        }

        var endpoint = await bus.GetSendEndpoint(new Uri($"queue:{queue}"));
        
        using (StreamReader stream = new StreamReader(context.Request.Body))
        {
            var body = await stream.ReadToEndAsync();

            logger.LogInformation("Sending message with {Body} to message queue", body);

            // todo: get the tenant in an earlier middleware
            await endpoint.Send(new Message { Body = body, Tenant = tenantProvider.Tenant});
            
            logger.LogInformation("Sent message to message queue {Queue}", queue);
            // respond to client that their request has been accepted
            context.Response.StatusCode = (int)HttpStatusCode.Accepted;
        }
        // return out here without calling next(), this responds to the client
        // rather than continuing to handle the request
    }
}
