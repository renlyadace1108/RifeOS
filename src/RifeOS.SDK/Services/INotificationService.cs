using RifeOS.SDK.Models;

namespace RifeOS.SDK.Services;

public interface INotificationService
{
    Task ShowAsync(NotificationMessage notification, CancellationToken cancellationToken = default);
}
