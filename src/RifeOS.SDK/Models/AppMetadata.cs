using RifeOS.SDK.Enums;

namespace RifeOS.SDK.Models;

public sealed record AppMetadata(
    string Id,
    string Name,
    string Description,
    string Version = "1.0.0",
    string Author = "RifeOS",
    string Icon = "DefaultAppIcon",
    AppCategory Category = AppCategory.Tools
);