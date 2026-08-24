using RifeOS.SDK.Enums;

namespace RifeOS.SDK.Models;

public sealed record AppMetadata(
    string Id,
    string Name,
    string Version,
    string Description,
    string Icon,
    string Author,
    AppCategory Category
);
