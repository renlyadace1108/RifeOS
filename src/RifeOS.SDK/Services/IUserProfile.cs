namespace RifeOS.SDK.Services;

public interface IUserProfile
{
    string UserName { get; }
    string AvatarPath { get; }
    DateTime CreatedAt { get; }
}