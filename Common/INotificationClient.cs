namespace Common;

public interface INotificationClient
{
    Task NotifyAsync<T>(T content);
}