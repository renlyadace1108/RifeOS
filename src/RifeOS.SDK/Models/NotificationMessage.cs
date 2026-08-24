using RifeOS.SDK.Enums;

namespace RifeOS.SDK.Models;

public sealed record NotificationMessage(
    string Title,
    string Content,
    NotificationLevel Level = NotificationLevel.Info,
    int DurationSeconds = 4
);