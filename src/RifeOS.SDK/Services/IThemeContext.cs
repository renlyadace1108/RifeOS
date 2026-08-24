using RifeOS.SDK.Enums;

namespace RifeOS.SDK.Services;

public interface IThemeContext
{
    ThemeMode CurrentMode { get; }
    string PrimaryColorHex { get; }
    string BackgroundColorHex { get; }
    string ForegroundColorHex { get; }
    event EventHandler<ThemeMode>? ThemeChanged;
    void SetTheme(ThemeMode mode);
}
