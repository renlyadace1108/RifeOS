namespace RifeOS.SDK.Services;

public interface IUserProfile
{
    string UserId { get; }
    string Nickname { get; }
    string Avatar { get; }
    string Signature { get; }
}
