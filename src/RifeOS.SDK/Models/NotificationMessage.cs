using RifeOS.SDK.Enums;

namespace RifeOS.SDK.Models;

public sealed record NotificationMessage(
    string Title,
    string Content,
    NotificationType Type,
    string SourceAppId,
    int DurationMilliseconds = 3000
);
