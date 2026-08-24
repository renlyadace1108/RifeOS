using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RifeOS.SDK.Services;

namespace RifeOS.Host.Services;

public sealed class UserProfileService : IUserProfile
{
    private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "System", "user.json");

    public string UserId { get; private set; } = "user_master";
    public string Nickname { get; private set; } = "Renly";
    public string Avatar { get; private set; } = "pack://application:,,,/Themes/DefaultAvatar.png";
    public string Signature { get; private set; } = "Stay focused. Build the future.";
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsInitialized { get; private set; }

    public UserProfileService()
    {
        LoadProfile();
    }

    public void LoadProfile()
    {
        if (!File.Exists(ConfigPath))
        {
            IsInitialized = false;
            return;
        }

        try
        {
            var json = File.ReadAllText(ConfigPath);
            var doc = JsonDocument.Parse(json).RootElement;
            UserId = doc.TryGetProperty("UserId", out var uid) ? uid.GetString() ?? UserId : UserId;
            Nickname = doc.TryGetProperty("Nickname", out var nick) ? nick.GetString() ?? Nickname : Nickname;
            Avatar = doc.TryGetProperty("Avatar", out var av) ? av.GetString() ?? Avatar : Avatar;
            Signature = doc.TryGetProperty("Signature", out var sig) ? sig.GetString() ?? Signature : Signature;
            PasswordHash = doc.TryGetProperty("PasswordHash", out var p) ? p.GetString() ?? string.Empty : string.Empty;
            IsInitialized = doc.TryGetProperty("IsInitialized", out var init) && init.GetBoolean();
        }
        catch
        {
            IsInitialized = false;
        }
    }

    public void InitializeUser(string nickname, string password, string signature)
    {
        Nickname = string.IsNullOrWhiteSpace(nickname) ? "Renly" : nickname.Trim();
        Signature = string.IsNullOrWhiteSpace(signature) ? "Stay focused. Build the future." : signature.Trim();
        PasswordHash = HashPassword(password);
        IsInitialized = true;
        SaveProfile();
    }

    public void SaveProfile()
    {
        var dir = Path.GetDirectoryName(ConfigPath)!;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var data = new { UserId, Nickname, Avatar, Signature, PasswordHash, IsInitialized };
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }

    private static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password)) return string.Empty;
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}