using RifeOS.SDK.Services;

namespace RifeOS.Host.Services;

public sealed class UserProfileService : IUserProfile
{
    public static UserProfileService Instance { get; } = new();

    public string UserName { get; set; } = "Developer";
    public string AvatarPath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}