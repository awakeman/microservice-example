
using System.Net;
using MassTransit;
using Ocelot.Values;

namespace Gateway;

public class AsyncRequestHandler : IMiddleware
{
    private readonly IBus bus;

    public AsyncRequestHandler(IBus bus)
    {
        this.bus = bus;
    }

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

            // todo: get the tenant in an earlier middleware
            await endpoint.Send(new Message { Body = body, Tenant = "test" });
            
            // respond to client that their request has been accepted
            context.Response.StatusCode = (int)HttpStatusCode.Accepted;
        }
    }
}

public class Message 
{
    public required string Body { get; set; }
    public required string Tenant { get; set; }
}