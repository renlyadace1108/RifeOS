using System.Windows;
using System.Windows.Media;
using RifeOS.SDK.Enums;
using RifeOS.SDK.Services;

namespace RifeOS.Host.Services;

public sealed class ThemeService : IThemeContext
{
    public ThemeMode CurrentMode { get; private set; } = ThemeMode.Dark;
    public string PrimaryColorHex { get; private set; } = "#3B82F6";
    public string BackgroundColorHex { get; private set; } = "#121212";
    public string ForegroundColorHex { get; private set; } = "#EDEDED";

    public event EventHandler<ThemeMode>? ThemeChanged;

    public void SetTheme(ThemeMode mode)
    {
        CurrentMode = mode;
        if (mode == ThemeMode.Dark)
        {
            PrimaryColorHex = "#3B82F6";
            BackgroundColorHex = "#121212";
            ForegroundColorHex = "#EDEDED";
        }
        else
        {
            PrimaryColorHex = "#2563EB";
            BackgroundColorHex = "#F4F5F7";
            ForegroundColorHex = "#1F2328";
        }

        ApplyDynamicResources();
        ThemeChanged?.Invoke(this, mode);
    }

    private void ApplyDynamicResources()
    {
        if (Application.Current == null) return;

        Application.Current.Resources["PrimaryBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(PrimaryColorHex));
        Application.Current.Resources["BackgroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundColorHex));
        Application.Current.Resources["ForegroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ForegroundColorHex));
        Application.Current.Resources["CardBackgroundBrush"] = new SolidColorBrush(CurrentMode == ThemeMode.Dark 
            ? (Color)ColorConverter.ConvertFromString("#1E1E1E") 
            : (Color)ColorConverter.ConvertFromString("#FFFFFF"));
        Application.Current.Resources["BorderBrush"] = new SolidColorBrush(CurrentMode == ThemeMode.Dark 
            ? (Color)ColorConverter.ConvertFromString("#2C2C2C") 
            : (Color)ColorConverter.ConvertFromString("#E1E4E8"));
    }
}
