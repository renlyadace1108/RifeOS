using RifeOS.SDK.Models;
using RifeOS.SDK.Services;

namespace RifeOS.Host.Services;

public sealed class NotificationService : INotificationService
{
    public event Action<NotificationMessage>? NotificationReceived;

    public Task ShowAsync(NotificationMessage notification, CancellationToken cancellationToken = default)
    {
        NotificationReceived?.Invoke(notification);
        return Task.CompletedTask;
    }
}
