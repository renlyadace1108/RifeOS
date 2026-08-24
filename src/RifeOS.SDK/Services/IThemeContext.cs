namespace RifeOS.SDK.Services;

public interface IThemeContext
{
    string CurrentTheme { get; }
    void SwitchTheme(string themeName);
    event Action<string>? ThemeChanged;
}