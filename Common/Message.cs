namespace Common;

public record Message 
{
    public required string Body { get; init; }
    public required string Tenant { get; init; }
}