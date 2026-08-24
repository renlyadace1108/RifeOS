using RifeOS.SDK.Enums;
using RifeOS.SDK.Models;

namespace RifeOS.SDK.Services;

public interface INotificationService
{
    void Show(string title, string content, NotificationLevel level = NotificationLevel.Info);
    Task ShowAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}